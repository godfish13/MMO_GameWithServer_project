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
            if (Data == null || Data.projectile == null || Owner == null || MyRoom == null)
                return;

            if (_nextMoveTick >= Environment.TickCount64)   // TickCount64 : millisecond 기준
                return;

            long tick = (long)(1000 / Data.projectile.speed);   // speed = 10으로 설정해줬으므로 == 0.01
            _nextMoveTick = Environment.TickCount64 + tick;     // 0.01초당 1칸씩 움직이도록 속도 조정

            Vector2Int destPos = GetFrontCellPos();
            if (MyRoom.Map.CanGo(destPos))
            {
                CellPos = destPos;

                S_Move movePacket = new S_Move();
                movePacket.ObjectId = Id;
                movePacket.PosInfo = PosInfo;
                MyRoom.BroadCast(movePacket);

                //Console.WriteLine("Arrow moving");
            }
            else
            {
                GameObject target = MyRoom.Map.FindObjectInCellPos(destPos);
                if (target != null)
                {
                    // Todo 피격판정
                    target.OnDamaged(this, Data.damage + Owner.Stat.Attack);    // Owner의 정보또한 Arrow내에 있으므로 this로 넘겨줌
                }
                // 소멸
                MyRoom.Push(MyRoom.LeaveGame, Id);
            }
        }       
    }
}
