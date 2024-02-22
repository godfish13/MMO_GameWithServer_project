using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MonsterCtrl : CreatureCtrl
{
    // Monster�� gameobject ����

    protected override void Init()
    {
        base.Init();
    }

    protected override void UpdateCtrl()
    {
        GetDirInput();
        base.UpdateCtrl();
    }

    private void LateUpdate()
    {
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }

    void GetDirInput()  // Ű �Է� �� �̵������ �������� ����
    {
        if (State == CreatureState.Moving)  // �̵����� �ƴϸ� �Է��� ���ް� ����
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

        if (Managers.mapMgr.CanGo(destPos))     // �̵� ������ ��ǥ���� üũ �� �̵�
        {
            _cellPos = destPos;
        }
        State = CreatureState.Moving;   // ���� �����ִ� Arrived ���¿����� �ִϸ��̼� ������Ʈ �����ϵ��� �̵�Ű �Է¹����� �ϴ� Moving���� ����
    }
}
