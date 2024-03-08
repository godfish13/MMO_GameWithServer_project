using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using UnityEngine;
using static Define;

public class CreatureCtrl : MonoBehaviour
{
    // Player, Monster, etc ... ctrl base class

    public float _speed = 5.0f;
    protected float yoffset = 0.5f;   // Cell ĭ �߾ӿ� ��������Ʈ ��ġ�� �µ��� ������ ���� 

    [SerializeField] private Vector3Int _cellPos = Vector3Int.zero; // �����ؼ� CellPos get set������ serializeField�ᵵ inspector������ ������ �ȵ� �׷��� ���� �� ����
    public Vector3Int CellPos { get { return _cellPos; } set { _cellPos = value; } }
    protected Animator _animator;
    protected SpriteRenderer _spriteRenderer;

    [SerializeField] protected CreatureState _state = CreatureState.Idle;
    public virtual CreatureState State
    {
        get { return _state; }
        set                         // �̵� �� ��ų��� �� ���º� �ִϸ��̼� ������Ʈ
        {
            if (_state == value)
                return;

            _state = value;
            UpdateAnim();
        }
    }

    protected MoveDir _lastDir = MoveDir.Down;    // ������ Idle Anim ��� ������ �����ϱ� ���� ���������� �ٶ� ���� �����
    [SerializeField] protected MoveDir _dir = MoveDir.Down;    // �ʱ���°� ���� �ٶ󺸴� ���·� ����  
    public MoveDir Dir      // ���� ���� ������ �ִϸ��̼��� ���ÿ� ����ǵ��� ������Ƽ ����
    {
        get { return _dir; }
        set                         // Idle Anim ������Ʈ
        {
            if (_dir == value)
                return;
            
            _dir = value;

            if (value != MoveDir.None)  // ���������� �ٶ� ���� ���
                _lastDir = value;

            UpdateAnim();   // _dir ��ȭ �� �ִϸ��̼� ������Ʈ
        }
    }

    public Vector3Int GetFrontCellPos()
    {
        Vector3Int cellPos = CellPos;

        switch (_lastDir)
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
            switch (_lastDir)
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
            switch (_dir) 
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
            switch (_lastDir)
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
        if (dist < _speed * Time.deltaTime) // �̵��Ÿ��� �ѹ��� �̵� ������ �Ÿ����� ª����� �����ߴٰ� ����
        {           
            transform.position = destPos;
            CalculateDestPos();             // �Էµ� Ű�� ���� ���� ��ǥ ���� ����           
        }
        else
        {
            transform.position += moveDir.normalized * _speed * Time.deltaTime;
            State = CreatureState.Moving;
        }
    }

    protected virtual void CalculateDestPos()   // �Էµ� Ű�� ���� ���� ��ǥ ���� ���� 
    {
        if (_dir == MoveDir.None)
        {
            State = CreatureState.Idle;
            return;
        }

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

        if (Managers.mapMgr.CanGo(destPos))     // �̵� ������ ��ǥ���� üũ �� �̵�
        {
            if (Managers.objectMgr.SearchPos(destPos) == null)
            {
                CellPos = destPos;
            }           
        } 
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
