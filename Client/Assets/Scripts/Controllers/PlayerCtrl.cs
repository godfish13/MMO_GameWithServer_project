using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class PlayerCtrl : MonoBehaviour
{
    //GameObject (Player) ����

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
        Vector3 pos = Managers.mapMgr.CurrentGrid.CellToWorld(_cellPos) + new Vector3(0.5f, 0.5f);    // CellToWorld : �� ��ǥ�� ������ǥ�� ��ȯ����
        transform.position = pos;
        _animator = gameObject.GetComponent<Animator>();
    }

    private void Update()
    {
        GetDirInput();
        UpdatePosition();
    }

    private void LateUpdate()
    {
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }

    void GetDirInput()  // Ű �Է� �� �̵������ �������� ����
    {
        if (_isMoving == true)  // �ѹ��� ��ĭ���� �̵��ϰ� �̵� �� �ִϸ��̼� ������Ʈ �ȵǵ��� _isMoving ����
            return;

        Vector3Int destPos = _cellPos;

        if (Input.GetKey(KeyCode.W))
        {
            Dir = MoveDir.Up;
            destPos += Vector3Int.up;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            Dir = MoveDir.Down;
            destPos += Vector3Int.down;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            Dir = MoveDir.Left;
            destPos += Vector3Int.left;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            Dir = MoveDir.Right;
            destPos += Vector3Int.right;
        }
        else
        {
            Dir = MoveDir.None;
        }

        if (Managers.mapMgr.CanGo(destPos))     // �̵� ������ ��ǥ�̈� üũ �� �̵�
        {
            _cellPos = destPos;
            _isMoving = true;
        }
    }

    void UpdatePosition()   // Grid ��ĭ�� �̵��ϵ��� ����
    {
        if (_isMoving == false)
            return;

        Vector3 destpos = Managers.mapMgr.CurrentGrid.CellToWorld(_cellPos) + new Vector3(0.5f, 0.5f);    // CellToWorld : �� ��ǥ�� ������ǥ�� ��ȯ����
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
