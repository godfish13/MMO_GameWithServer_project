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
                    S_Spawn spawnPacket = new S_Spawn();
                    foreach (Player p in _players)  // 자신은 제외하고 현재 GameRoom에 입장해있는 플레이어들 정보 전송
                    {
                        if (newPlayer != p)
                            spawnPacket.PlayerList.Add(p.info);
                    }
                    newPlayer.mySession.Send(spawnPacket);
                }
                #endregion

                #region 타인한테 자신이 입장했다고 데이터 전송
                {
                    S_Spawn spawnPacket = new S_Spawn();
                    spawnPacket.PlayerList.Add(newPlayer.info);

                    foreach (Player player in _players) // 타 플레이어들에게 newPlayer입장사실 전달
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
    }
}
