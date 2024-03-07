using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MonsterCtrl : CreatureCtrl
{
    // Monster류 gameobject 부착
    Coroutine _coPatrol;
    Vector3Int _destCellPos;

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
        }
    }

    protected override void Init()
    {
        base.Init();
        _speed = 3.0f;

        State = CreatureState.Idle;
        Dir = MoveDir.None;
    }

    protected override void UpdateIdle()
    {
        base.UpdateIdle();
        
        if (_coPatrol == null)
        {
            _coPatrol = StartCoroutine("CoPatrol");
        }
    }

    protected override void CalculateDestPos()
    {
        //Todo : 목표지점까지 경로 계산 후 이동

        Vector3Int moveCellDir = _destCellPos - CellPos;

        if (moveCellDir.y > 0)
            Dir = MoveDir.Up;
        else if (moveCellDir.y < 0)
            Dir = MoveDir.Down;
        else if (moveCellDir.x > 0)
            Dir = MoveDir.Right;
        else if (moveCellDir.x < 0)
            Dir = MoveDir.Left;
        else
            Dir = MoveDir.None;

        Vector3Int destPos = CellPos;

        switch (_dir)
        {
            case MoveDir.Up:
                destPos += Vector3Int.up;
                break;
            case MoveDir.Down:
                destPos += Vector3Int.down;
                break;
            case MoveDir.Right:
                destPos += Vector3Int.right;
                break;
            case MoveDir.Left:
                destPos += Vector3Int.left;
                break;
        }

        if (Managers.mapMgr.CanGo(destPos) && Managers.objectMgr.SearchPos(destPos) == null)     // 이동 가능한 좌표인지 체크 후 이동
        {
            CellPos = destPos;   
        }
        else
        {
            State = CreatureState.Idle;
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
        Managers.objectMgr.Remove(gameObject);
        Managers.resourceMgr.Destroy(gameObject);
    }

    IEnumerator CoPatrol()
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
                _destCellPos = randPos;
                State = CreatureState.Moving;
                yield break;
            }
        }

        State = CreatureState.Idle; 
    }
}
