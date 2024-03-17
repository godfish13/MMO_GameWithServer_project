using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.InGame
{
    public class Player
    {
        public PlayerInfo info {  get; set; } = new PlayerInfo();
        public GameRoom myRoom { get; set; }    //플레이어가 현재 속해있는 GameRoom
        public ClientSession mySession { get; set; }
    }
}
