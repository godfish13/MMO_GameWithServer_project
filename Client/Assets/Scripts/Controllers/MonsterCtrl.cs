using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MonsterCtrl : CreatureCtrl
{
    // Monster�� gameobject ����
    Coroutine _coPatrol;
    Vector3Int _destCellPos;

    public override CreatureState State
    {
        get { return _state; }
        set                         // �̵� �� ��ų��� �� ���º� �ִϸ��̼� ������Ʈ
        {
            if (_state == value)
                return;

            base.State = value;
            if (_coPatrol != null)          // Idle���·� ���ư� �� �ٽ��ѹ� �ڷ�ƾ �����ϱ� ���� null�� �о���
            {                               // Idle�� ���ư������� _coPatrol�� �����ų �����̹Ƿ�
                StopCoroutine(_coPatrol);   // �ڷ�ƾ�� _coPatrol�� null�� �о��ִ°� �����ϴ� ���
                _coPatrol = null;           // ������Ƽ�� override�Ͽ� �����Ͽ� Idle�� �ɶ����� �˾Ƽ� �о��ֵ��� ����
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
        //Todo : ��ǥ�������� ��� ��� �� �̵�

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

        if (Managers.mapMgr.CanGo(destPos) && Managers.objectMgr.SearchPos(destPos) == null)     // �̵� ������ ��ǥ���� üũ �� �̵�
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
        //�ǰ� ����Ʈ ���
        GameObject de = Managers.resourceMgr.Instantiate("Effect/DeathBoom");   // ����Ʈ ���
        de.transform.position = transform.position;
        de.GetComponent<Animator>().Play("DeathBoom");
        Managers.resourceMgr.Destroy(de, 0.4f);

        // ��� �̤�
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

            if (Managers.mapMgr.CanGo(randPos) && Managers.objectMgr.SearchPos(randPos) == null)    // ������ġ CanGo, ������Ʈ ���� üũ
            {
                _destCellPos = randPos;
                State = CreatureState.Moving;
                yield break;
            }
        }

        State = CreatureState.Idle; 
    }
}
