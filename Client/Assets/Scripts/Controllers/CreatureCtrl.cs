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

    [SerializeField] private Vector3Int _CellPosition = new Vector3Int();   // inspector 내에서 CellPos 관찰을 위한 디버그용 따로 작동에 쓰진않음

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
    public float Speed      // 자주사용할듯하므로 하나 빼놔줌
    {
        get { return Stat.Speed; }
        set { Stat.Speed = value; }
    }

    protected float yoffset = 0.5f;   // Cell 칸 중앙에 스프라이트 위치가 맞도록 개별로 지정 

    protected bool _updated = false;  // 더티 플래그 : 실제로 업데이트 되었는지 체크하기 위해 둔 변수
                                      // C_Move를 보내는 조건으로 CellPos, State, Dir이 변했는지 체크하기 위해 설정

    private PositionInfo _positionInfo = new PositionInfo();
    public PositionInfo PosInfo             // PositionInfo에 State, Dir, X, Y 정보 모두 담겨있으므로 이걸로 일원화
    {
        get { return _positionInfo; } 
        set
        {
            if (_positionInfo.Equals(value))    // positionInfo에 변화가 생길때만 Set
                return;

            State = value.State;        // _lastDir 갱신등이 씹히는 문제 해결하기위해 _positionInfo를 통으로 처리하는 대신 각자 값 넣어주도록 변경
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

            _CellPosition = CellPos;    // inspector 내에서 CellPos 관찰을 위한 디버그용 따로 작동에 쓰진않음
        } 
    }

    protected Animator _animator;
    protected SpriteRenderer _spriteRenderer;

    public virtual CreatureState State
    {
        get { return PosInfo.State; }
        set                         // 이동 및 스킬사용 등 상태별 애니메이션 업데이트
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
    public MoveDir Dir      // 현재 상태 설정과 애니메이션을 동시에 변경되도록 프로퍼티 설정
    {
        get { return PosInfo.MoveDir; }
        set                         // Idle Anim 업데이트
        {
            if (PosInfo.MoveDir == value)
                return;

            PosInfo.MoveDir = value;

            UpdateAnim();   // _dir 변화 시 애니메이션 업데이트
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

    protected virtual void UpdateAnim()   // Arrow 등 Animation이 없는 Ctrl에서 내용물 없게 override 할 수 있도록 virtual로 선언
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
                    _spriteRenderer.flipX = true;  // SpriteRenderer의 flipX값으로 스프라이트 좌우반전
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

    protected virtual void UpdateMoving()   // Grid 한칸씩 이동하도록 구현
    {
        //if (State != CreatureState.Moving)    이런식으로 예외처리 하는 대신 위 UpdateCtrl에서 case에 따라 작동하도록 변경하여 관리 용이하게 함
            //return;

        Vector3 destPos = Managers.mapMgr.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, yoffset);    // CellToWorld : 셀 좌표를 월드좌표로 변환해줌
        Vector3 moveDir = destPos - transform.position;       

        // 도착 여부 체크
        float dist = moveDir.magnitude;
        if (dist < Speed * Time.deltaTime) // 이동거리가 한번에 이동 가능한 거리보다 짧은경우 도착했다고 인정
        {           
            transform.position = destPos;
            CalculateDestPos();             // 입력된 키에 따른 다음 목표 지점 지정           
        }
        else
        {
            transform.position += moveDir.normalized * Speed * Time.deltaTime;
            State = CreatureState.Moving;
        }
    }

    protected virtual void CalculateDestPos()   // 입력된 키에 따른 다음 목표 지점 지정 
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
