using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MonsterCtrl : CreatureCtrl
{
    // Monster�� gameobject ����
    Coroutine _coPatrol;
    Coroutine _coSearchPlayer;      // �÷��̾� Ž�� �ڷ�ƾ
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
        // Chase (A*)��ǥ�������� ��� ��� �� �̵�
       
        if (_target != null)    // Ÿ��(�÷��̾�)�� ������ �÷��̾� ����
        {
            destPos = _target.GetComponent<CreatureCtrl>().CellPos;

            Vector3Int dir2target = destPos - CellPos;
            if (dir2target.magnitude <= _skillRange && (dir2target.x == 0 || dir2target.y == 0))    // ��ų�������̰� �÷��̾�� �������̸� ����
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
        if (path.Count < 2 || (_target != null && path.Count > 20)) // path�� [0]�� �ڽ��̹Ƿ� Count�� 1�̸� path�� �̻�(����)�ϴٴ� ���̹Ƿ� �ʱ�ȭ
        {                                      // _target������ path�� �ʹ� �־�����(�÷��̾ 20���� �ָ� ��������) �ʱ�ȭ
            _target = null;
            State = CreatureState.Idle;
            return;
        }

        //if (_target != null)
        //    Debug.Log(path[1]);

        Vector3Int nextPos = path[1];   // _target������ path�� 1�ʸ��� �׸��Ƿ� �Ź� path[1]�� ��ǥ�� �̵�      
        Vector3Int moveCellDir = nextPos - CellPos;

        Dir = GetDirfromVector(moveCellDir);

        if (Managers.mapMgr.CanGo(nextPos) && Managers.objectMgr.SearchPos(nextPos) == null)  // �̵� ������ ��ǥ���� üũ �� �̵�
        {
            CellPos = nextPos;
            //Debug.Log("CanGo!");
        }
        else
        {
            State = CreatureState.Idle;
            //Debug.Log("����!");
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

            if (Managers.mapMgr.CanGo(randPos) && Managers.objectMgr.SearchPos(randPos) == null)    // ������ġ CanGo, ������Ʈ ���� üũ
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

            _target = Managers.objectMgr.FindGameObject((go) =>   // PlayerCtrl�� �޷��ִ� Player ������Ʈ Ž��    
            {
                PlayerCtrl pc = go.GetComponent<PlayerCtrl>();  
                if (pc == null)
                    return false;

                Vector3Int dir = pc.CellPos - CellPos;  // Player ������Ʈ �߰������� �Ÿ�����
                if (dir.magnitude > _searchRange)
                    return false;

                return true;
            });
        }
    }

    IEnumerator coPunchSkill()
    {
        // �ǰ� ����
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

        // ��� �ð�
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

        // �ǰ������� ArrowCtrl���� ����

        // ��� �ð�
        yield return new WaitForSeconds(0.4f);
        State = CreatureState.Moving;
        _coSkill = null;
    }
}
