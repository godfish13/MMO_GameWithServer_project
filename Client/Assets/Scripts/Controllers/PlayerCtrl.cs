using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class PlayerCtrl : MonoBehaviour
{
    //GameObject (Player) ����

    public Grid _grid;
    public float _speed = 5.0f;

    [SerializeField] private Vector3Int _cellPos = Vector3Int.zero;
    private bool _isMoving = false;
    Animator _animator;

    private MoveDir _dir = MoveDir.Down;    // �ʱ���°� ���� �ٶ󺸴� ���·� ����
    public MoveDir Dir      // ���� ���� ������ �ִϸ��̼��� ���ÿ� ����ǵ��� ������Ƽ ����
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
                    transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);  // localScale x�� -1�� ������ �¿������
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
                    else                                // default ���µ� FRONT�� ����
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
        Vector3 pos = _grid.CellToWorld(_cellPos) + new Vector3(0.5f, 0.5f);    // CellToWorld : �� ��ǥ�� ������ǥ�� ��ȯ����
        transform.position = pos;
        _animator = gameObject.GetComponent<Animator>();
    }

    private void Update()
    {
        GetDirInput();
        UpdatePosition();
    }

    void GetDirInput()  // Ű �Է� �� �̵������ �������� ����
    {
        if (_isMoving == true)  // �ѹ��� ��ĭ���� �̵��ϰ� �̵� �� �ִϸ��̼� ������Ʈ �ȵǵ��� _isMoving ����
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

    void UpdatePosition()   // Grid ��ĭ�� �̵��ϵ��� ����
    {
        if (_isMoving == false)
            return;

        Vector3 destpos = _grid.CellToWorld(_cellPos) + new Vector3(0.5f, 0.5f);    // CellToWorld : �� ��ǥ�� ������ǥ�� ��ȯ����
        Vector3 moveDir = destpos - transform.position;

        // ���� ���� üũ
        float dist = moveDir.magnitude;
        if (dist < _speed * Time.deltaTime) // �̵��Ÿ��� �ѹ��� �̵� ������ �Ÿ����� ª����� �����ߴٰ� ����
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
