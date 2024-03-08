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
    protected float yoffset = 0.5f;   // Cell 칸 중앙에 스프라이트 위치가 맞도록 개별로 지정 

    [SerializeField] private Vector3Int _cellPos = Vector3Int.zero; // 단축해서 CellPos get set만쓰면 serializeField써도 inspector내에서 관찰이 안됨 그래서 굳이 또 써줌
    public Vector3Int CellPos { get { return _cellPos; } set { _cellPos = value; } }
    protected Animator _animator;
    protected SpriteRenderer _spriteRenderer;

    [SerializeField] protected CreatureState _state = CreatureState.Idle;
    public virtual CreatureState State
    {
        get { return _state; }
        set                         // 이동 및 스킬사용 등 상태별 애니메이션 업데이트
        {
            if (_state == value)
                return;

            _state = value;
            UpdateAnim();
        }
    }

    protected MoveDir _lastDir = MoveDir.Down;    // 순전히 Idle Anim 재생 방향을 결정하기 위해 마지막으로 바라본 방향 저장용
    [SerializeField] protected MoveDir _dir = MoveDir.Down;    // 초기상태가 앞을 바라보는 상태로 설정  
    public MoveDir Dir      // 현재 상태 설정과 애니메이션을 동시에 변경되도록 프로퍼티 설정
    {
        get { return _dir; }
        set                         // Idle Anim 업데이트
        {
            if (_dir == value)
                return;
            
            _dir = value;

            if (value != MoveDir.None)  // 마지막으로 바라본 방향 기록
                _lastDir = value;

            UpdateAnim();   // _dir 변화 시 애니메이션 업데이트
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

    protected virtual void UpdateAnim()   // Arrow 등 Animation이 없는 Ctrl에서 내용물 없게 override 할 수 있도록 virtual로 선언
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
                    _spriteRenderer.flipX = true;  // SpriteRenderer의 flipX값으로 스프라이트 좌우반전
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
        Vector3 pos = Managers.mapMgr.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, yoffset);    // CellToWorld : 셀 좌표를 월드좌표로 변환해줌
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

    protected virtual void UpdateMoving()   // Grid 한칸씩 이동하도록 구현
    {
        //if (State != CreatureState.Moving)    이런식으로 예외처리 하는 대신 위 UpdateCtrl에서 case에 따라 작동하도록 변경하여 관리 용이하게 함
            //return;

        Vector3 destPos = Managers.mapMgr.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, yoffset);    // CellToWorld : 셀 좌표를 월드좌표로 변환해줌
        Vector3 moveDir = destPos - transform.position;       

        // 도착 여부 체크
        float dist = moveDir.magnitude;
        if (dist < _speed * Time.deltaTime) // 이동거리가 한번에 이동 가능한 거리보다 짧은경우 도착했다고 인정
        {           
            transform.position = destPos;
            CalculateDestPos();             // 입력된 키에 따른 다음 목표 지점 지정           
        }
        else
        {
            transform.position += moveDir.normalized * _speed * Time.deltaTime;
            State = CreatureState.Moving;
        }
    }

    protected virtual void CalculateDestPos()   // 입력된 키에 따른 다음 목표 지점 지정 
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

        if (Managers.mapMgr.CanGo(destPos))     // 이동 가능한 좌표인지 체크 후 이동
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
