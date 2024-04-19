using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    internal class Monster : GameObject
    {
        public Monster()
        {
            ObjectType = GameObjectType.Monster;

            // Tmp 하드코딩 추후 Json연동
            Stat.Level = 1;
            Stat.Hp = 100;
            Stat.MaxHp = 100;
            Stat.Speed = 3.0f;

            State = CreatureState.Idle;
        }

        // FSM 방식 AI
        public override void Update()
        {
            switch (State)
            { 
                case CreatureState.Idle:
                    UpdateIdle();
                    break;
                case CreatureState.Moving:
                    UpdateMoving();
                    break;
                case CreatureState.Skill:
                    UpdateSkill();
                    break;
                case CreatureState.Dead:
                    UpdateDead();
                    break;
            }
        }

        #region Update series
        Player _target; // 일단은 참조값으로 들고있기 / target 찾으면 해당 플레이어의 id를 대신 갖고있는거도 괜춘할듯

        int _searchDist = 10;
        long _nextSearchTick = 0;   // 탐색 주기 틱
        protected virtual void UpdateIdle() 
        {
            if (_nextSearchTick > Environment.TickCount64)
                return;
            _nextSearchTick = Environment.TickCount64 + 1000;   // 1ms기준이므로 1000 = 1초

            Player target = MyRoom.FindPlayer(p =>
            {
                Vector2Int dir = p.CellPos - this.CellPos;              
                return dir.CellDistFromZero < _searchDist;
            });

            if (target == null)
                return;

            _target = target;
            State = CreatureState.Moving;
        }

        long _nextMoveTick = 0;
        int _maxChaseDist = 20;
        protected virtual void UpdateMoving()
        {
            if (_nextMoveTick > Environment.TickCount64)
                return;

            int moveTick = (int)((1000) / Speed);               // Speed = 5.0f로 1초에 5칸 움직이도록 설정됨
            _nextMoveTick = Environment.TickCount64 + moveTick; // 그러므로 1초 == 1000ms, 1000/5 == 200ms마다 1칸씩 움직이도록 설정

            if (_target == null || _target.MyRoom != MyRoom)    // 플레이어가 나가거나 다른 지역(gameroom)으로 가버리면 Idle로 돌아가기
            {
                _target = null;
                State = CreatureState.Idle;
                return;
            }

            int dist = (_target.CellPos - CellPos).CellDistFromZero;
            if (dist == 0 || dist > _maxChaseDist)
            {
                _target = null;
                State = CreatureState.Idle;
                return;
            }

            List<Vector2Int> path = MyRoom.Map.FindPath(this.CellPos, _target.CellPos, CheckObjectsInPath: false);
            if (path.Count < 2 || path.Count > _maxChaseDist)   // path에 자신의 현재 CellPos만 있거나 / 너무 멀리떨어져있거나
            {
                _target = null;
                State = CreatureState.Idle;
                return;
            }

            // 이동
            Dir = GetDirfromVector(path[1] - this.CellPos);
            MyRoom.Map.ApplyMove(this, path[1]);

            // Client들에게 몬스터가 움직인 사실 전달
            S_Move movePacket = new S_Move();
            movePacket.ObjectId = this.Id;
            movePacket.PosInfo = this.PosInfo;
            this.MyRoom.BroadCast(movePacket);
        }

        protected virtual void UpdateSkill()
        {

        }

        protected virtual void UpdateDead()
        {

        }
        #endregion
    }
}
