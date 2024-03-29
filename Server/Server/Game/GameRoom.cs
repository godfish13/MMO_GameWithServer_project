﻿using Google.Protobuf;
using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.InGame
{
    public class GameRoom
    {
        object _lock = new object();
        public int RoomId { get; set; }

        List<Player> _players = new List<Player>(); // 해당 룸에 접속중인 player들

        public void EnterGame(Player newPlayer)
        {
            if (newPlayer == null)
                return;            
                
            lock (_lock)
            {
                _players.Add(newPlayer);
                newPlayer.myRoom = this;

                #region player 입장 성공시 Client의 player 본인에게 데이터 전송
                {
                    S_EnterGame enterPacket = new S_EnterGame();    // player 본인의 Info 전송
                    enterPacket.Player = newPlayer.info;
                    newPlayer.mySession.Send(enterPacket);

                    S_Spawn spawnPacket = new S_Spawn(); // 자신은 제외하고 현재 GameRoom에 입장해있는 플레이어들 정보 전송
                    foreach (Player p in _players)  
                    {
                        if (newPlayer != p)
                            spawnPacket.PlayerList.Add(p.info);
                    }
                    newPlayer.mySession.Send(spawnPacket);
                }
                #endregion

                #region 타인한테 자신이 입장했다고 데이터 전송
                {
                    S_Spawn spawnPacket = new S_Spawn();    // 타 플레이어들에게 newPlayer입장사실 전달
                    spawnPacket.PlayerList.Add(newPlayer.info);

                    foreach (Player player in _players) 
                    {
                        if (newPlayer != player)
                            player.mySession.Send(spawnPacket);
                    }
                }
                #endregion
            }
        }

        public void LeaveGame(int playerId) 
        {
            lock (_lock)
            {
                Player player = _players.Find(player => player.info.PlayerId == playerId);
                if (player == null)
                    return;

                _players.Remove(player);
                player.myRoom = null;

                #region player 퇴장 성공시 Client의 player 본인에게 데이터 전송
                {
                    S_LeaveGame leavePacket = new S_LeaveGame();
                    player.mySession.Send(leavePacket);
                }
                #endregion


                #region 타인한테 player가 퇴장했다고 데이터 전송
                {
                    S_Despawn despawnPacket = new S_Despawn();
                    despawnPacket.PlayerIdList.Add(player.info.PlayerId);
                    foreach (Player p in _players)
                    {
                        if (player != p)
                            p.mySession.Send(despawnPacket);
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
                // 서버에 저장된 자신의 좌표 변경(이동)
                PlayerInfo info = player.info;
                info.PosInfo = movePacket.PosInfo;

                // 다른 플레이어들에게 자기위치 방송
                S_Move broadMovePkt = new S_Move(); // 방송하려고 서버측에서 보내는 M 패킷
                broadMovePkt.PlayerId = player.info.PlayerId;   // 움직인 자신 Id 입력
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
                PlayerInfo info = player.info;
                if (info.PosInfo.State != CreatureState.Idle)   // Idle상태에서만 스킬쓰도록 했음
                    return;

                // Todo 스킬 사용 가능 여부 체크

                info.PosInfo.State = CreatureState.Skill;

                S_Skill broadSkillPacket = new S_Skill() { SkillInfo = new SkillInfo() };
                broadSkillPacket.PlayerId = player.info.PlayerId;
                broadSkillPacket.SkillInfo.SkillId = 1;     // Todo 데이터 시트로 나중에 변경 예정 일단 간단히 1만 설정
                BroadCast(broadSkillPacket);

                // Todo 데미지 판정
            }
        }

        public void BroadCast(IMessage packet)
        {
             lock (_lock)
            {
                foreach (Player p in _players)
                {
                    p.mySession.Send(packet);
                }
            }
        }
    }
}
