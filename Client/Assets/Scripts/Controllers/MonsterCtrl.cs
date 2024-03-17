using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MonsterCtrl : CreatureCtrl
{
    // Monster류 gameobject 부착
    Coroutine _coPatrol;
    Coroutine _coSearchPlayer;      // 플레이어 탐색 코루틴
    Coroutine _coSkill;

    [SerializeField] Vector3Int _randomdestCellPos;
    [SerializeField] GameObject _target;
    [SerializeField] float _searchRange = 10.0f;
    [SerializeField] float _skillRange = 1.0f;

    [SerializeField] Vector3Int destPos = new Vector3Int();

    public bool isRange { get; set; } = false;

    public override CreatureState State
    {
        get { return _state; }
        set                         // 이동 및 스킬사용 등 상태별 애니메이션 업데이트
        {
            if (_state == value)
                return;

            base.State = value;
            if (_coPatrol != null)          // Idle상태로 돌아간 후 다시한번 코루틴 실행하기 위해 null로 밀어줌
            {                               // Idle로 돌아갈때마다 _coPatrol을 재생시킬 예정이므로
                StopCoroutine(_coPatrol);   // 코루틴에 _coPatrol을 null로 밀어주는걸 구현하는 대신
                _coPatrol = null;           // 프로퍼티를 override하여 구현하여 Idle이 될때마다 알아서 밀어주도록 구현
            }

            if (_coSearchPlayer != null)
            {                        
                StopCoroutine(_coSearchPlayer);
                _coSearchPlayer = null; 
            }
        }
    }

    protected override void Init()
    {
        base.Init();
        _speed = 3.0f;

        State = CreatureState.Idle;
        Dir = MoveDir.None;

        if (isRange)
            _skillRange = 10.0f;
    }

    protected override void UpdateIdle()
    {
        base.UpdateIdle();
        
        if (_coPatrol == null)
        {
            _coPatrol = StartCoroutine("coPatrol");
        }

        if (_coSearchPlayer == null)
        {
            _coSearchPlayer = StartCoroutine("coSearchPlayer");
        }
    }

    protected override void CalculateDestPos()
    {
        // Chase (A*)목표지점까지 경로 계산 후 이동
       
        if (_target != null)    // 타겟(플레이어)가 있으면 플레이어 추적
        {
            destPos = _target.GetComponent<CreatureCtrl>().CellPos;

            Vector3Int dir2target = destPos - CellPos;
            if (dir2target.magnitude <= _skillRange && (dir2target.x == 0 || dir2target.y == 0))    // 스킬범위안이고 플레이어와 일직선이면 공격
            {
                Dir = GetDirfromVector(dir2target);
                State = CreatureState.Skill;
                if (isRange)
                {
                    _coSkill = StartCoroutine("coRockSkill");
                }
                else
                {
                    _coSkill = StartCoroutine("coPunchSkill");
                }

                return;
            }
        }
        else
        {
            destPos = _randomdestCellPos;   // Patrol
        }

        List<Vector3Int> path = Managers.mapMgr.FindPath(CellPos, destPos, ignoreDestCollision : true);
        if (path.Count < 2 || (_target != null && path.Count > 20)) // path는 [0]이 자신이므로 Count가 1이면 path가 이상(없다)하다는 뜻이므로 초기화
        {                                      // _target까지의 path가 너무 멀어지면(플레이어가 20보다 멀리 떨어지면) 초기화
            _target = null;
            State = CreatureState.Idle;
            return;
        }

        //if (_target != null)
        //    Debug.Log(path[1]);

        Vector3Int nextPos = path[1];   // _target까지의 path를 1초마다 그리므로 매번 path[1]를 목표로 이동      
        Vector3Int moveCellDir = nextPos - CellPos;

        Dir = GetDirfromVector(moveCellDir);

        if (Managers.mapMgr.CanGo(nextPos) && Managers.objectMgr.SearchPos(nextPos) == null)  // 이동 가능한 좌표인지 체크 후 이동
        {
            CellPos = nextPos;
            //Debug.Log("CanGo!");
        }
        else
        {
            State = CreatureState.Idle;
            //Debug.Log("못가!");
        }
    }

    public override void OnDamaged()
    {
        //피격 이펙트 재생
        GameObject de = Managers.resourceMgr.Instantiate("Effect/DeathBoom");   // 이펙트 재생
        de.transform.position = transform.position;
        de.GetComponent<Animator>().Play("DeathBoom");
        Managers.resourceMgr.Destroy(de, 0.4f);

        // 사망 ㅜㅜ
        Managers.objectMgr.Remove(Id);
        Managers.resourceMgr.Destroy(gameObject);
    }

    IEnumerator coPatrol()
    {
        int waitSeconds = Random.Range(1, 4);
        yield return new WaitForSeconds(waitSeconds);

        for (int i = 0; i < 10; i++)
        {
            int xRange = Random.Range(-3, 3);
            int yRange = Random.Range(-3, 3);
            Vector3Int randPos = CellPos + new Vector3Int(xRange, yRange, 0);

            if (Managers.mapMgr.CanGo(randPos) && Managers.objectMgr.SearchPos(randPos) == null)    // 랜덤위치 CanGo, 오브젝트 유무 체크
            {
                _randomdestCellPos = randPos;
                State = CreatureState.Moving;
                yield break;
            }
        }

        State = CreatureState.Idle; 
    }

    IEnumerator coSearchPlayer()
    {
        while (true)
        {
            yield return new WaitForSeconds(1.0f);

            if (_target != null)
                continue;

            _target = Managers.objectMgr.FindGameObject((go) =>   // PlayerCtrl이 달려있는 Player 오브젝트 탐색    
            {
                PlayerCtrl pc = go.GetComponent<PlayerCtrl>();  
                if (pc == null)
                    return false;

                Vector3Int dir = pc.CellPos - CellPos;  // Player 오브젝트 발견했으면 거리측정
                if (dir.magnitude > _searchRange)
                    return false;

                return true;
            });
        }
    }

    IEnumerator coPunchSkill()
    {
        // 피격 판정
        GameObject go = Managers.objectMgr.SearchPos(GetFrontCellPos());
        if (go != null)
        {
            Debug.Log(go.name);

            CreatureCtrl cc = go.GetComponent<CreatureCtrl>();
            if (cc != null)
            {
                cc.OnDamaged();
            }
        }

        // 대기 시간
        yield return new WaitForSeconds(0.5f);
        State = CreatureState.Moving;
        _coSkill = null;
    }

    IEnumerator coRockSkill()
    {
        GameObject go = Managers.resourceMgr.Instantiate("Creature/Rock");
        ArrowCtrl ac = go.GetComponent<ArrowCtrl>();
        ac.Dir = _lastDir;
        ac.CellPos = CellPos;

        // 피격판정은 ArrowCtrl에서 관리

        // 대기 시간
        yield return new WaitForSeconds(0.4f);
        State = CreatureState.Moving;
        _coSkill = null;
    }
}
