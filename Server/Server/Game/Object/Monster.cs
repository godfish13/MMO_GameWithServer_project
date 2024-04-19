using Google.Protobuf.Protocol;
using Server.Data;
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
        int _skillRange = 1;
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
                BroadCastMove();    // Idle상태 전달
                return;
            }

            Vector2Int dir = _target.CellPos - this.CellPos;
            int dist = dir.CellDistFromZero;
            if (dist == 0 || dist > _maxChaseDist)
            {
                _target = null;
                State = CreatureState.Idle;
                BroadCastMove();    // Idle상태 전달
                return;
            }

            List<Vector2Int> path = MyRoom.Map.FindPath(this.CellPos, _target.CellPos, CheckObjectsInPath: false);
            if (path.Count < 2 || path.Count > _maxChaseDist)   // path에 자신의 현재 CellPos만 있거나 / 너무 멀리떨어져있거나
            {
                _target = null;
                State = CreatureState.Idle;
                BroadCastMove();    // Idle상태 전달
                return;
            }

            #region 스킬 사용가능 여부(사거리안) 판단
            if (dist <= _skillRange && (dir.x == 0 || dir.y == 0))  // x, y 둘다 1이면 대각선으로 스킬씀 대각선없는 게임이므로 조건에서 제외
            {
                _coolDownTick = 0;
                State = CreatureState.Skill;
                return;
            }
            #endregion

            // 이동
            Dir = GetDirfromVector(path[1] - this.CellPos);
            MyRoom.Map.ApplyMove(this, path[1]);

            BroadCastMove();    // Client들에게 몬스터가 움직인 사실 전달
        }

        void BroadCastMove()
        {
            // Client들에게 몬스터가 움직인 사실 전달
            S_Move movePacket = new S_Move();
            movePacket.ObjectId = this.Id;
            movePacket.PosInfo = this.PosInfo;
            this.MyRoom.BroadCast(movePacket);
        }

        long _coolDownTick = 0;
        protected virtual void UpdateSkill()
        {
            if (_coolDownTick == 0)
            {
                // 타겟이 유효한지 체크
                if (_target == null || _target.MyRoom != this.MyRoom || _target.Hp == 0)
                {
                    _target = null;
                    State = CreatureState.Moving;
                    BroadCastMove();
                    return;
                }

                // 스킬 사용가능한 상황인지 체크
                Vector2Int dir = (_target.CellPos - this.CellPos);
                int dist = dir.CellDistFromZero;
                bool canUseSkill = dist <= _skillRange && (dir.x == 0 || dir.y == 0);
                if (canUseSkill == false) 
                {
                    State = CreatureState.Moving;   // 플레이어가 그새 멀어졌으면 다시 쫓아가도록
                    BroadCastMove();
                    return;
                }

                // 타겟팅 방향 주시
                MoveDir lookDir = GetDirfromVector(dir);
                if (Dir != lookDir)
                {
                    Dir = lookDir;
                    BroadCastMove();
                }

                Skill skillData = null;
                DataMgr.SkillDictionary.TryGetValue(1, out skillData);

                // 데미지 판정
                _target.OnDamaged(this, skillData.damage + this.Stat.Attack);   // 판정 패킷은 OnDamaged에서 알아서 보내줌

                // 스킬 사용 packet broadCast
                S_Skill skillPacket = new S_Skill() { SkillInfo = new SkillInfo() };
                skillPacket.ObjectId = this.Id;
                skillPacket.SkillInfo.SkillId = skillData.id;
                MyRoom.BroadCast(skillPacket);

                // 스킬 쿨타임 측정시작
                int coolTick = (int)(1000 * skillData.cooldown);
                _coolDownTick = Environment.TickCount64 + coolTick;
            }

            // 스킬 쿨타임 적용
            if (_coolDownTick > Environment.TickCount64)
                return;

            // 쿨타임 초기화
            _coolDownTick = 0;
        }

        protected virtual void UpdateDead()
        {

        }
        #endregion
    }
}
