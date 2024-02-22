using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static Define;

public class CreatureCtrl : MonoBehaviour
{
    // Player, Monster, etc ... ctrl base class

    public float _speed = 5.0f;

    [SerializeField] protected private Vector3Int _cellPos = Vector3Int.zero;
    protected Animator _animator;
    protected SpriteRenderer _spriteRenderer;

    [SerializeField] protected CreatureState _state = CreatureState.Idle;
    public CreatureState State
    {
        get { return _state; }
        set
        {
            if (_state == value)
                return;

            _state = value;

            UpdateAnim();   // _state ��ȭ �� �ִϸ��̼� ������Ʈ
        }
    }

    [SerializeField] private MoveDir _dir = MoveDir.Down;    // �ʱ���°� ���� �ٶ󺸴� ���·� ����
    private MoveDir _lastDir = MoveDir.Down;    // ������ Idle Anim ��� ������ �����ϱ� ���� ���������� �ٶ� ���� �����
    public MoveDir Dir      // ���� ���� ������ �ִϸ��̼��� ���ÿ� ����ǵ��� ������Ƽ ����
    {
        get { return _dir; }
        set
        {
            if (_dir == value)
                return;
            
            _dir = value;

            if (value != MoveDir.None)  // ���������� �ٶ� ���� ���
                _lastDir = _dir;

            UpdateAnim();   // _dir ��ȭ �� �ִϸ��̼� ������Ʈ
        }
    }

    protected void UpdateAnim()
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
            //Todo
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
        Vector3 pos = Managers.mapMgr.CurrentGrid.CellToWorld(_cellPos) + new Vector3(0.5f, 0.7f);    // CellToWorld : �� ��ǥ�� ������ǥ�� ��ȯ����
        transform.position = pos;
        _animator = gameObject.GetComponent<Animator>();
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    protected virtual void UpdateCtrl()
    {
        UpdatePosition();
    }

    void UpdatePosition()   // Grid ��ĭ�� �̵��ϵ��� ����
    {
        if (State != CreatureState.Moving)
            return;

        Vector3 destpos = Managers.mapMgr.CurrentGrid.CellToWorld(_cellPos) + new Vector3(0.5f, 0.7f);    // CellToWorld : �� ��ǥ�� ������ǥ�� ��ȯ����
        Vector3 moveDir = destpos - transform.position;

        // ���� ���� üũ
        float dist = moveDir.magnitude;
        if (dist < _speed * Time.deltaTime) // �̵��Ÿ��� �ѹ��� �̵� ������ �Ÿ����� ª����� �����ߴٰ� ����
        {
            transform.position = destpos;
            State = CreatureState.Arrived;  // ��ǥ������ ���������� ���� �Է��� ���� ��� Arrived ����
            if (_dir == MoveDir.None)       // ��ǥ���� �����ϰ� �Է��� ���� ��� Idle ���·� ����
                State = CreatureState.Idle;
        }
        else
        {
            transform.position += moveDir.normalized * _speed * Time.deltaTime;
            State = CreatureState.Moving;
        }
    }
}
