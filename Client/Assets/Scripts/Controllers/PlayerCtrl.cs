using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class PlayerCtrl : CreatureCtrl
{
    //GameObject (Player) ����

    protected override void Init()
    {
        base.Init();
    }

    protected override void UpdateCtrl()
    {        
        base.UpdateCtrl();
        GetDirInput();
    }

    private void LateUpdate()
    {
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }

    void GetDirInput()  // Ű �Է� �� �̵������ �������� ����
    {
        if (State == CreatureState.Moving)  // �̵����� �ƴϸ� �Է��� ���ް� ����
            return;

        Vector3Int destPos = CellPos;

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
            if (Managers.objectMgr.SearchPos(destPos) == null)
            {
                CellPos = destPos;
            }
        }
        State = CreatureState.Moving;   // ���� �����ִ� Arrived ���¿����� �ִϸ��̼� ������Ʈ �����ϵ��� �̵�Ű �Է¹����� �ϴ� Moving���� ����
    }
}
