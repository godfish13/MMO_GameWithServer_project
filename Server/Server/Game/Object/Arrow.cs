using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server.Game
{
    internal class Arrow : Projectile
    {
        public GameObject Owner { get; set; }   // 화살 쏜 주인

        long _nextMoveTick = 0;


        public override void Update()
        {
            if (Owner == null || MyRoom == null)
                return;

            if (_nextMoveTick >= Environment.TickCount64)
                return;

            _nextMoveTick = Environment.TickCount64 + 50;

            Vector2Int destPos = GetFrontCellPos();
            if (MyRoom.Map.CanGo(destPos))
            {
                CellPos = destPos;

                S_Move movePacket = new S_Move();
                movePacket.ObjectId = ObjectId;
                movePacket.PosInfo = PosInfo;
                MyRoom.BroadCast(movePacket);

                Console.WriteLine("Arrow moving");
            }
            else
            {
                GameObject target = MyRoom.Map.FindObjectInCellPos(destPos);
                if (target != null)
                {
                    // Todo 피격판정
                
                }
                // 소멸
                MyRoom.LeaveGame(ObjectId);
            }
        }
    }
}
