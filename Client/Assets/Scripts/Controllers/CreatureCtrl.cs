using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using UnityEngine;

public class CreatureCtrl : MonoBehaviour
{
    // Player, Monster, etc ... ctrl base class

    [SerializeField] private int _id;
    public int Id { get { return _id; } set { _id = value; } }

    [SerializeField] private Vector3Int _CellPosition = new Vector3Int();   // inspector ������ CellPos ������ ���� ����׿� ���� �۵��� ��������

    StatInfo _stat = new StatInfo();
    public StatInfo Stat
    {
        get { return _stat; }
        set
        {
            if (_stat.Equals(value))
                return;

            _stat.Hp = value.Hp;
            _stat.MaxHp = value.MaxHp;
            _stat.Speed = value.Speed;
        }
    }
    public float Speed      // ���ֻ���ҵ��ϹǷ� �ϳ� ������
    {
        get { return Stat.Speed; }
        set { Stat.Speed = value; }
    }

    protected float yoffset = 0.5f;   // Cell ĭ �߾ӿ� ��������Ʈ ��ġ�� �µ��� ������ ���� 

    protected bool _updated = false;  // ��Ƽ �÷��� : ������ ������Ʈ �Ǿ����� üũ�ϱ� ���� �� ����
                                      // C_Move�� ������ �������� CellPos, State, Dir�� ���ߴ��� üũ�ϱ� ���� ����

    private PositionInfo _positionInfo = new PositionInfo();
    public PositionInfo PosInfo             // PositionInfo�� State, Dir, X, Y ���� ��� ��������Ƿ� �̰ɷ� �Ͽ�ȭ
    {
        get { return _positionInfo; } 
        set
        {
            if (_positionInfo.Equals(value))    // positionInfo�� ��ȭ�� ���涧�� Set
                return;

            State = value.State;        // _lastDir ���ŵ��� ������ ���� �ذ��ϱ����� _positionInfo�� ������ ó���ϴ� ��� ���� �� �־��ֵ��� ����
            Dir = value.MoveDir;
            CellPos = new Vector3Int(value.PosX, value.PosY, 0);
        }
    }

    public void SyncPos()
    {
        Vector3 destPos = Managers.mapMgr.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, yoffset);
        transform.position = destPos;
    }

    public Vector3Int CellPos 
    {
        get { return new Vector3Int(PosInfo.PosX, PosInfo.PosY, 0); } 
        set 
        {
            if (PosInfo.PosX == value.x && PosInfo.PosY == value.y)
                return;
            PosInfo.PosX = value.x; 
            PosInfo.PosY = value.y;           
            _updated = true;

            _CellPosition = CellPos;    // inspector ������ CellPos ������ ���� ����׿� ���� �۵��� ��������
        } 
    }

    protected Animator _animator;
    protected SpriteRenderer _spriteRenderer;

    public virtual CreatureState State
    {
        get { return PosInfo.State; }
        set                         // �̵� �� ��ų��� �� ���º� �ִϸ��̼� ������Ʈ
        {
            if (PosInfo.State == value)
                return;

            PosInfo.State = value;
            UpdateAnim();
            _updated = true;
        }
    }

    [SerializeField] private MoveDir _dir 
    {
        get { return _dir; }
        set
        {
            _dir = value;
        }
    }
    public MoveDir Dir      // ���� ���� ������ �ִϸ��̼��� ���ÿ� ����ǵ��� ������Ƽ ����
    {
        get { return PosInfo.MoveDir; }
        set                         // Idle Anim ������Ʈ
        {
            if (PosInfo.MoveDir == value)
                return;

            PosInfo.MoveDir = value;

            UpdateAnim();   // _dir ��ȭ �� �ִϸ��̼� ������Ʈ
            _updated = true;
        }
    }  

    public MoveDir GetDirfromVector(Vector3Int dir)
    {
        if (dir.y > 0)
            return MoveDir.Up;      
        else if (dir.x > 0)
            return MoveDir.Right;
        else if (dir.x < 0)
            return MoveDir.Left;
        else
            return MoveDir.Down;
    }

    public Vector3Int GetFrontCellPos()
    {
        Vector3Int cellPos = CellPos;

        switch (Dir)
        {
            case MoveDir.Up:
                cellPos += Vector3Int.up;
                break;
            case MoveDir.Down:
                cellPos += Vector3Int.down;
                break;
            case MoveDir.Right:
                cellPos += Vector3Int.right;
                break;
            case MoveDir.Left:
                cellPos += Vector3Int.left;
                break;
        }

        return cellPos;
    }

    protected virtual void UpdateAnim()   // Arrow �� Animation�� ���� Ctrl���� ���빰 ���� override �� �� �ֵ��� virtual�� ����
    {
        if (State == CreatureState.Idle)
        {
            switch (Dir)
            {
                case MoveDir.Up:
                    _animator.Play("IDLE_BACK");
                    _spriteRenderer.flipX = false;
                    break;
                case MoveDir.Down:
                    _animator.Play("IDLE_FRONT");
                    _spriteRenderer.flipX = false;
                    break;
                case MoveDir.Right:
                    _animator.Play("IDLE_RIGHT");
                    _spriteRenderer.flipX = false;
                    break;
                case MoveDir.Left:
                    _animator.Play("IDLE_RIGHT");
                    _spriteRenderer.flipX = true;
                    break;
            }
        }        
        else if (State == CreatureState.Moving)
        {
            switch (Dir) 
            {
                case MoveDir.Up:
                    _animator.Play("WALK_BACK");
                    _spriteRenderer.flipX = false;
                    break;
                case MoveDir.Down:
                    _animator.Play("WALK_FRONT");
                    _spriteRenderer.flipX = false;
                    break;
                case MoveDir.Right:
                    _animator.Play("WALK_RIGHT");
                    _spriteRenderer.flipX = false;
                    break;
                case MoveDir.Left:
                    _animator.Play("WALK_RIGHT");
                    _spriteRenderer.flipX = true;  // SpriteRenderer�� flipX������ ��������Ʈ �¿����
                    break;
            }
        }
        else if (State == CreatureState.Skill)
        {
            switch (Dir)
            {
                case MoveDir.Up:
                    _animator.Play("ATTACK_BACK");
                    _spriteRenderer.flipX = false;
                    break;
                case MoveDir.Down:
                    _animator.Play("ATTACK_FRONT");
                    _spriteRenderer.flipX = false;
                    break;
                case MoveDir.Right:
                    _animator.Play("ATTACK_RIGHT");
                    _spriteRenderer.flipX = false;
                    break;
                case MoveDir.Left:
                    _animator.Play("ATTACK_RIGHT");
                    _spriteRenderer.flipX = true;  // SpriteRenderer�� flipX������ ��������Ʈ �¿����
                    break;
            }
        }
        else
        {
            //Todo
        }
    }

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        UpdateCtrl();
    }

    protected virtual void Init()
    {
        Vector3 pos = Managers.mapMgr.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, yoffset);    // CellToWorld : �� ��ǥ�� ������ǥ�� ��ȯ����
        transform.position = pos;
        _animator = gameObject.GetComponent<Animator>();
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        State = CreatureState.Idle;
        Dir = MoveDir.Down;
        UpdateAnim();
    }

    protected virtual void UpdateCtrl()
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

    protected virtual void UpdateIdle()
    {

    }

    protected virtual void UpdateMoving()   // Grid ��ĭ�� �̵��ϵ��� ����
    {
        //if (State != CreatureState.Moving)    �̷������� ����ó�� �ϴ� ��� �� UpdateCtrl���� case�� ���� �۵��ϵ��� �����Ͽ� ���� �����ϰ� ��
            //return;

        Vector3 destPos = Managers.mapMgr.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, yoffset);    // CellToWorld : �� ��ǥ�� ������ǥ�� ��ȯ����
        Vector3 moveDir = destPos - transform.position;       

        // ���� ���� üũ
        float dist = moveDir.magnitude;
        if (dist < Speed * Time.deltaTime) // �̵��Ÿ��� �ѹ��� �̵� ������ �Ÿ����� ª����� �����ߴٰ� ����
        {           
            transform.position = destPos;
            CalculateDestPos();             // �Էµ� Ű�� ���� ���� ��ǥ ���� ����           
        }
        else
        {
            transform.position += moveDir.normalized * Speed * Time.deltaTime;
            State = CreatureState.Moving;
        }
    }

    protected virtual void CalculateDestPos()   // �Էµ� Ű�� ���� ���� ��ǥ ���� ���� 
    {
        
    }

    protected virtual void UpdateSkill()
    {

    }

    protected virtual void UpdateDead()
    {

    }

    public virtual void OnDamaged()
    {
        
    }
}
