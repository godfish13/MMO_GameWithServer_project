using Google.Protobuf;
using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Server.Game
{
    public class GameRoom
    {
        object _lock = new object();
        public int RoomId { get; set; }

        Dictionary<int, Player> _players = new Dictionary<int, Player>(); // 해당 룸에 접속중인 player들
        Dictionary<int, Monster> _monsters = new Dictionary<int, Monster>();
        Dictionary<int, Projectile> _projectiles = new Dictionary<int, Projectile>();

        public Map Map { get; private set; } = new Map();

        public void Init(int mapId)
        {
            Map.LoadMap(mapId);
        }

        public void Update()
        {
            lock (_lock)
            {
                foreach (Projectile p in _projectiles.Values) 
                {
                    p.Update();
                }
            }
        }

        public void EnterGame(GameObject newObject)
        {
            if (newObject == null)
                return;

            GameObjectType type = ObjectMgr.GetObjectTypebyId(newObject.ObjectId);
            
            lock (_lock)
            {
                if (type == GameObjectType.Player)
                {
                    Player newPlayer = newObject as Player;
                    _players.Add(newPlayer.ObjectId, newPlayer);
                    newPlayer.MyRoom = this;

                    #region player 입장 성공시 Client의 player 본인에게 데이터 전송 
                    // player 본인의 Info 전송
                    S_EnterGame enterPacket = new S_EnterGame();    
                    enterPacket.Player = newPlayer.Info;
                    newPlayer.mySession.Send(enterPacket);

                    // 자신은 제외하고 현재 GameRoom에 입장해있는 플레이어들 정보 전송
                    S_Spawn OthersSPacket = new S_Spawn(); 
                    foreach (Player p in _players.Values)
                    {
                        if (newPlayer != p)
                            OthersSPacket.ObjectList.Add(p.Info);
                    }
                    newPlayer.mySession.Send(OthersSPacket);                   
                    #endregion
                }
                else if (type == GameObjectType.Monster)
                {
                    Monster newMonster = newObject as Monster;
                    _monsters.Add(newMonster.ObjectId, newMonster);
                    newMonster.MyRoom = this;
                }
                else if (type == GameObjectType.Projectile) 
                {
                    Projectile newProjectile = newObject as Projectile;
                    _projectiles.Add(newProjectile.ObjectId, newProjectile);
                    newProjectile.MyRoom = this;
                }

                #region 모두에게 입장한 오브젝트 spawn시키라고 데이터 전송                
                S_Spawn spawnPacket = new S_Spawn();
                spawnPacket.ObjectList.Add(newObject.Info);

                foreach (Player player in _players.Values)
                {
                    if (player.ObjectId != newObject.ObjectId)
                        player.mySession.Send(spawnPacket);
                }              
                #endregion
            }
        }

        public void LeaveGame(int objectId)
        {
            GameObjectType type = ObjectMgr.GetObjectTypebyId(objectId);

            lock (_lock)
            {
                if (type == GameObjectType.Player)
                {
                    Player player = null;
                    if (_players.Remove(objectId, out player) == false)
                        return;

                    player.MyRoom = null;
                    Map.ApplyLeave(player);

                    #region player 퇴장 성공시 Client의 player 본인에게 데이터 전송
                    {
                        S_LeaveGame leavePacket = new S_LeaveGame();
                        player.mySession.Send(leavePacket);
                    }
                    #endregion
                }
                else if (type == GameObjectType.Monster)
                {
                    Monster monster = null;
                    if (_monsters.Remove(objectId, out monster) == false)
                        return;

                    monster.MyRoom = null;
                    Map.ApplyLeave(monster);
                }
                else if (type == GameObjectType.Projectile)
                {
                    Projectile projectile = null;
                    if (_projectiles.Remove(objectId, out projectile) == false)
                        return;

                    projectile.MyRoom = null;
                }

                #region 타인한테 player가 퇴장했다고 데이터 전송
                {
                    S_Despawn despawnPacket = new S_Despawn();
                    despawnPacket.ObjectIdList.Add(objectId);
                    foreach (Player player in _players.Values)
                    {
                        if (player.ObjectId != objectId)
                            player.mySession.Send(despawnPacket);
                    }
                }
                #endregion
            }
        }

        public void HandleMove(Player player, C_Move movePacket)
        {
            if (player == null)
                return;

            lock (_lock)
            {
                // move 패킷 정상 검증
                PositionInfo movePosInfo = movePacket.PosInfo;
                ObjectInfo info = player.Info;
                if (movePosInfo.PosX != info.PosInfo.PosX || movePosInfo.PosY != info.PosInfo.PosY) // 현 좌표랑 목표좌표랑 다른지 체크
                {
                    if (Map.CanGo(new Vector2Int(movePosInfo.PosX, movePosInfo.PosY)) == false)
                        return;
                }
                // 서버에 저장된 자신의 좌표 변경(이동)
                info.PosInfo.State = movePosInfo.State;
                info.PosInfo.MoveDir = movePosInfo.MoveDir;
                Map.ApplyMove(player, new Vector2Int(movePosInfo.PosX, movePosInfo.PosY));

                // 다른 플레이어들에게 자기위치 방송
                S_Move broadMovePkt = new S_Move(); // 방송하려고 서버측에서 보내는 M 패킷
                broadMovePkt.ObjectId = player.Info.ObjectId;   // 움직인 자신 Id 입력
                broadMovePkt.PosInfo = movePacket.PosInfo;

                BroadCast(broadMovePkt);
            }
        }

        public void HandleSkill(Player player, C_Skill skillPacket)
        {
            if (player == null)
                return;

            lock (_lock)
            {
                ObjectInfo info = player.Info;
                if (info.PosInfo.State != CreatureState.Idle)   // Idle상태에서만 스킬쓰도록 했음
                    return;

                // Todo 스킬 사용 가능 여부 체크

                info.PosInfo.State = CreatureState.Skill;
                S_Skill broadSkillPacket = new S_Skill() { SkillInfo = new SkillInfo() };
                broadSkillPacket.ObjectId = player.Info.ObjectId;
                broadSkillPacket.SkillInfo.SkillId = skillPacket.SkillInfo.SkillId;     // Todo 데이터 시트로 나중에 변경 예정 일단 간단히 1만 설정
                BroadCast(broadSkillPacket);

                // 데미지 판정
                if (skillPacket.SkillInfo.SkillId == 1)         // punch
                {
                    Vector2Int skillTargetPos = player.GetFrontCellPos(info.PosInfo.MoveDir);
                    GameObject target = Map.FindObjectInCellPos(skillTargetPos);
                    if (target != null)
                    {
                        Console.WriteLine($"Hitted GameObject {target.ObjectId}");
                    }
                }
                else if (skillPacket.SkillInfo.SkillId == 2)    // arrow
                {
                    Arrow arrow = ObjectMgr.Instance.Add<Arrow>();
                    if (arrow == null)
                        return;

                    arrow.Owner = player;
                    arrow.PosInfo.State = CreatureState.Moving;
                    arrow.PosInfo.MoveDir = player.PosInfo.MoveDir;
                    arrow.PosInfo.PosX = player.PosInfo.PosX;
                    arrow.PosInfo.PosY = player.PosInfo.PosY;

                    EnterGame(arrow);
                }
            }
        }

        public void BroadCast(IMessage packet)
        {
            lock (_lock)
            {
                foreach (Player p in _players.Values)
                {
                    p.mySession.Send(packet);
                }
            }
        }
    }
}
