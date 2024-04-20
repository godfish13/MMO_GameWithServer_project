using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server.Data;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Server.Game
{
    public class GameRoom : JobSerializer
    {
        public int RoomId { get; set; }

        Dictionary<int, Player> _players = new Dictionary<int, Player>(); // 해당 룸에 접속중인 player들
        Dictionary<int, Monster> _monsters = new Dictionary<int, Monster>();
        Dictionary<int, Projectile> _projectiles = new Dictionary<int, Projectile>();
        
        public Map Map { get; private set; } = new Map();

        public void Init(int mapId)
        {
            Map.LoadMap(mapId);

            // tmp monster init
            Monster monster = ObjectMgr.Instance.Add<Monster>();
            monster.Info.Name = "Monster";
            monster.CellPos = new Vector2Int(-5, -5);
            Push(EnterGame, monster);
        }

        public void Update()
        {
            foreach (Monster m in _monsters.Values)
            {
                m.Update();
            }

            foreach (Projectile p in _projectiles.Values) 
            {
                p.Update();
            }
        }

        public void EnterGame(GameObject newObject)
        {
            if (newObject == null)
                return;

            GameObjectType type = ObjectMgr.GetObjectTypebyId(newObject.Id);          
            
            if (type == GameObjectType.Player)
            {
                Player newPlayer = newObject as Player;
                _players.Add(newPlayer.Id, newPlayer);
                newPlayer.MyRoom = this;

                Map.ApplyMove(newPlayer, new Vector2Int(newPlayer.CellPos.x, newPlayer.CellPos.y)); // 접속순간 처음 위치로 초기화
                                                                                    // 안해주면 위치 인식이 안되서 안움직이면 collider판정 안됨
                #region player 입장 성공시 Client의 player 본인에게 데이터 전송 
                // player 본인의 Info 전송
                S_EnterGame enterPacket = new S_EnterGame();    
                enterPacket.Player = newPlayer.Info;
                newPlayer.mySession.Send(enterPacket);

                // 자신은 제외하고 현재 GameRoom에 입장해있는 플레이어들 정보 전송
                S_Spawn OthersSPacket = new S_Spawn(); 
                foreach (Player p in _players.Values)
                {
                    if (newPlayer != p)     // 자신 제외
                        OthersSPacket.ObjectList.Add(p.Info);
                }

                foreach (Monster m in _monsters.Values)
                    OthersSPacket.ObjectList.Add(m.Info);

                foreach (Projectile p in _projectiles.Values)
                    OthersSPacket.ObjectList.Add(p.Info);

                newPlayer.mySession.Send(OthersSPacket);                   
                #endregion
            }
            else if (type == GameObjectType.Monster)
            {
                Monster newMonster = newObject as Monster;
                _monsters.Add(newMonster.Id, newMonster);
                newMonster.MyRoom = this;
                Map.ApplyMove(newMonster, new Vector2Int(newMonster.CellPos.x, newMonster.CellPos.y));
            }
            else if (type == GameObjectType.Projectile) 
            {
                Projectile newProjectile = newObject as Projectile;
                _projectiles.Add(newProjectile.Id, newProjectile);
                newProjectile.MyRoom = this;
            }

            #region 모두에게 입장한 오브젝트 spawn시키라고 데이터 전송                
            S_Spawn spawnPacket = new S_Spawn();
            spawnPacket.ObjectList.Add(newObject.Info);

            foreach (Player player in _players.Values)
            {
                if (player.Id != newObject.Id)
                    player.mySession.Send(spawnPacket);
            }              
            #endregion           
        }

        public void LeaveGame(int objectId)
        {
            GameObjectType type = ObjectMgr.GetObjectTypebyId(objectId);
          
            if (type == GameObjectType.Player)
            {
                Player player = null;
                if (_players.Remove(objectId, out player) == false)
                    return;
                
                Map.ApplyLeave(player);
                player.MyRoom = null;   // ApplyLeave에서 MyRoom == null 이면 return되므로 ApplyLeave부터 실행 후 null

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
                    if (player.Id != objectId)
                        player.mySession.Send(despawnPacket);
                }
            }
            #endregion
            
        }

        public void HandleMove(Player player, C_Move movePacket)
        {
            if (player == null)
                return;
            
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

        public void HandleSkill(Player player, C_Skill skillPacket)
        {
            if (player == null)
                return;

            ObjectInfo info = player.Info;
            if (info.PosInfo.State != CreatureState.Idle)   // Idle상태에서만 스킬쓰도록 했음
                return;

            // Todo 스킬 사용 가능 여부 체크

            info.PosInfo.State = CreatureState.Skill;
            S_Skill broadSkillPacket = new S_Skill() { SkillInfo = new SkillInfo() };
            broadSkillPacket.ObjectId = player.Info.ObjectId;
            broadSkillPacket.SkillInfo.SkillId = skillPacket.SkillInfo.SkillId;
            BroadCast(broadSkillPacket);

            // Json 데이터 읽어와서 설정
            Data.Skill skillData = null;
            if (DataMgr.SkillDictionary.TryGetValue(skillPacket.SkillInfo.SkillId, out skillData) == false)
                return;

            switch (skillData.skillType)
            {
                // 데미지 판정
                case SkillType.SkillAuto:
                    {
                        Vector2Int skillTargetPos = player.GetFrontCellPos(info.PosInfo.MoveDir);
                        GameObject target = Map.FindObjectInCellPos(skillTargetPos);
                        if (target != null)
                        {
                            target.OnDamaged(player, player.Stat.Attack);
                            Console.WriteLine($"Hitted GameObject {ObjectMgr.GetDecimalId(target.Id)}");
                        }
                    }
                    break;
                case SkillType.SkillProjectile:
                    {
                        Arrow arrow = ObjectMgr.Instance.Add<Arrow>();
                        if (arrow == null)
                            return;

                        arrow.Owner = player;
                        arrow.Data = skillData;

                        arrow.PosInfo.State = CreatureState.Moving;
                        arrow.PosInfo.MoveDir = player.PosInfo.MoveDir;
                        arrow.PosInfo.PosX = player.PosInfo.PosX;
                        arrow.PosInfo.PosY = player.PosInfo.PosY;
                        arrow.Stat.Speed = skillData.projectile.speed;

                        Push(EnterGame, arrow);
                    }
                    break;
            }       
        }

        public Player FindPlayer(Func<GameObject, bool> condition)  // 원시적으로 플레이어 전부 탐색, condition에 맞는 player return
        {
            foreach (Player p in _players.Values)
            {
                if (condition.Invoke(p))
                    return p;
            }
            return null;
        }

        public void BroadCast(IMessage packet)
        {            
            foreach (Player p in _players.Values)
            {
                p.mySession.Send(packet);
            }           
        }
    }
}
