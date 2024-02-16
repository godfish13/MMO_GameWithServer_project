using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class PlayerCtrl : MonoBehaviour
{
    //GameObject (Player) 부착

    public Grid _grid;
    public float _speed = 5.0f;

    [SerializeField] private Vector3Int _cellPos = Vector3Int.zero;
    private bool _isMoving = false;
    Animator _animator;

    private MoveDir _dir = MoveDir.Down;    // 초기상태가 앞을 바라보는 상태로 설정
    public MoveDir Dir      // 현재 상태 설정과 애니메이션을 동시에 변경되도록 프로퍼티 설정
    {
        get { return _dir; }
        set
        {
            if (_dir == value)
                return;
            switch (value)
            {
                case MoveDir.Up:
                    _animator.Play("WALK_BACK");
                    transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                    break;
                case MoveDir.Down:
                    _animator.Play("WALK_FRONT");
                    transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                    break;
                case MoveDir.Right:
                    _animator.Play("WALK_RIGHT");
                    transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                    break;
                case MoveDir.Left:
                    _animator.Play("WALK_RIGHT");
                    transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);  // localScale x값 -1로 넣으면 좌우반전됨
                    break;
                case MoveDir.None:
                    if (_dir == MoveDir.Up)
                    {
                        _animator.Play("IDLE_BACK");
                        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                    }
                    else if (_dir == MoveDir.Right)
                    {
                        _animator.Play("IDLE_RIGHT");
                        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                    }
                    else if (_dir == MoveDir.Left)
                    {
                        _animator.Play("IDLE_RIGHT");
                        transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
                    }
                    else                                // default 상태도 FRONT로 설정
                    {
                        _animator.Play("IDLE_FRONT");
                        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                    }
                    break;
            }
            _dir = value;
        }
    }

    private void Start()
    {
        Vector3 pos = _grid.CellToWorld(_cellPos) + new Vector3(0.5f, 0.5f);    // CellToWorld : 셀 좌표를 월드좌표로 변환해줌
        transform.position = pos;
        _animator = gameObject.GetComponent<Animator>();
    }

    private void Update()
    {
        GetDirInput();
        UpdatePosition();
    }

    void GetDirInput()  // 키 입력 시 이동방향과 도착지점 설정
    {
        if (_isMoving == true)  // 한번에 한칸씩만 이동하고 이동 중 애니메이션 업데이트 안되도록 _isMoving 설정
            return;
        
        if (Input.GetKey(KeyCode.W))
        {
            Dir = MoveDir.Up;
            _cellPos += Vector3Int.up;
            _isMoving = true;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            Dir = MoveDir.Down;
            _cellPos += Vector3Int.down;
            _isMoving = true;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            Dir = MoveDir.Left;
            _cellPos += Vector3Int.left;
            _isMoving = true;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            Dir = MoveDir.Right;
            _cellPos += Vector3Int.right;
            _isMoving = true;
        }
        else
        {
            Dir = MoveDir.None;
        }
    }

    void UpdatePosition()   // Grid 한칸씩 이동하도록 구현
    {
        if (_isMoving == false)
            return;

        Vector3 destpos = _grid.CellToWorld(_cellPos) + new Vector3(0.5f, 0.5f);    // CellToWorld : 셀 좌표를 월드좌표로 변환해줌
        Vector3 moveDir = destpos - transform.position;

        // 도착 여부 체크
        float dist = moveDir.magnitude;
        if (dist < _speed * Time.deltaTime) // 이동거리가 한번에 이동 가능한 거리보다 짧은경우 도착했다고 인정
        {
            transform.position = destpos;
            _isMoving = false;
        }
        else
        {
            transform.position += moveDir.normalized * _speed * Time.deltaTime;
        }
    }
}
