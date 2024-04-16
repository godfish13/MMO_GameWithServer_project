using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    public class GameObject
    {
        public GameObjectType ObjectType { get; protected set; } = GameObjectType.None;
        public int Id
        {
            get { return Info.ObjectId; }
            set { Info.ObjectId = value; }
        }

        public GameRoom MyRoom { get; set; }    //플레이어가 현재 속해있는 GameRoom

        public ObjectInfo Info { get; set; } = new ObjectInfo();
        public PositionInfo PosInfo { get; private set; } = new PositionInfo();
        public StatInfo Stat { get; private set; } = new StatInfo();

        public float Speed
        {
            get { return Stat.Speed; }
            set { Stat.Speed = value; }
        }

        public GameObject() 
        {
            Info.PosInfo = PosInfo;
            Info.StatInfo = Stat;
        }

        public Vector2Int CellPos
        {
            get
            {
                return new Vector2Int(PosInfo.PosX, PosInfo.PosY);
            }
            set
            {
                Info.PosInfo.PosX = value.x;
                Info.PosInfo.PosY = value.y;
            }
        }

        public Vector2Int GetFrontCellPos()     // 바로 자기앞 Cell Pos 가져오도록 wrap해둠 // Arrow 전진 등에 사용하기 편하게
        {
            return GetFrontCellPos(PosInfo.MoveDir);
        }

        public Vector2Int GetFrontCellPos(MoveDir dir)
        {
            Vector2Int cellPos = CellPos;

            switch (dir)
            {
                case MoveDir.Up:
                    cellPos += Vector2Int.up;
                    break;
                case MoveDir.Down:
                    cellPos += Vector2Int.down;
                    break;
                case MoveDir.Right:
                    cellPos += Vector2Int.right;
                    break;
                case MoveDir.Left:
                    cellPos += Vector2Int.left;
                    break;
            }

            return cellPos;
        }

        public virtual void OnDamaged(GameObject attacker, int damage)
        {
            Stat.Hp = Math.Max(Stat.Hp - damage, 0);    // 체력이 0보다 작아지지않도록 Math.Max로 한줄로 가능

            S_ChangeHp changeHpPacket = new S_ChangeHp();   // Hp 변동 패킷 방송
            changeHpPacket.ObjectId = Id;
            changeHpPacket.Hp = Stat.Hp;
            changeHpPacket.DeltaHp = damage;
            MyRoom.BroadCast(changeHpPacket);

            if (Stat.Hp <= 0)
            {
                Stat.Hp = 0;
                OnDead(attacker);
            }
        }

        public virtual void OnDead(GameObject attacker)
        {
            S_OnDead diePacket = new S_OnDead();
            diePacket.ObjectId = this.Id;
            diePacket.AttackerId = attacker.Id;
            MyRoom.BroadCast(diePacket);

            GameRoom room = MyRoom; // LeaveGame에서 MyRoom = null 되므로 잠깐 긁어와서 사용하기위해 tmp로 하나 선언
            room.LeaveGame(Id);   // 일단 추방

            // 오브젝트 상태 리셋
            Stat.Hp = Stat.MaxHp;   
            PosInfo.State = CreatureState.Idle;
            PosInfo.MoveDir = MoveDir.Down;
            PosInfo.PosX = 0;
            PosInfo.PosY = 0;

            room.EnterGame(this);   // 리셋 후 재접속
        }
    }
}
