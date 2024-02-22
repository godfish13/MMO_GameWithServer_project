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

            UpdateAnim();   // _state 변화 시 애니메이션 업데이트
        }
    }

    [SerializeField] private MoveDir _dir = MoveDir.Down;    // 초기상태가 앞을 바라보는 상태로 설정
    private MoveDir _lastDir = MoveDir.Down;    // 순전히 Idle Anim 재생 방향을 결정하기 위해 마지막으로 바라본 방향 저장용
    public MoveDir Dir      // 현재 상태 설정과 애니메이션을 동시에 변경되도록 프로퍼티 설정
    {
        get { return _dir; }
        set
        {
            if (_dir == value)
                return;
            
            _dir = value;

            if (value != MoveDir.None)  // 마지막으로 바라본 방향 기록
                _lastDir = _dir;

            UpdateAnim();   // _dir 변화 시 애니메이션 업데이트
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
                    _spriteRenderer.flipX = true;  // SpriteRenderer의 flipX값으로 스프라이트 좌우반전
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
        Vector3 pos = Managers.mapMgr.CurrentGrid.CellToWorld(_cellPos) + new Vector3(0.5f, 0.7f);    // CellToWorld : 셀 좌표를 월드좌표로 변환해줌
        transform.position = pos;
        _animator = gameObject.GetComponent<Animator>();
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    protected virtual void UpdateCtrl()
    {
        UpdatePosition();
    }

    void UpdatePosition()   // Grid 한칸씩 이동하도록 구현
    {
        if (State != CreatureState.Moving)
            return;

        Vector3 destpos = Managers.mapMgr.CurrentGrid.CellToWorld(_cellPos) + new Vector3(0.5f, 0.7f);    // CellToWorld : 셀 좌표를 월드좌표로 변환해줌
        Vector3 moveDir = destpos - transform.position;

        // 도착 여부 체크
        float dist = moveDir.magnitude;
        if (dist < _speed * Time.deltaTime) // 이동거리가 한번에 이동 가능한 거리보다 짧은경우 도착했다고 인정
        {
            transform.position = destpos;
            State = CreatureState.Arrived;  // 목표지점에 도달했지만 아직 입력이 있을 경우 Arrived 상태
            if (_dir == MoveDir.None)       // 목표지점 도달하고 입력이 없을 경우 Idle 상태로 진입
                State = CreatureState.Idle;
        }
        else
        {
            transform.position += moveDir.normalized * _speed * Time.deltaTime;
            State = CreatureState.Moving;
        }
    }
}
