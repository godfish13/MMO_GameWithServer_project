using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class PlayerCtrl : CreatureCtrl
{
    //GameObject (Player) 부착

    protected override void Init()
    {
        base.Init();
    }

    protected override void UpdateCtrl()
    {        
        base.UpdateCtrl();

        switch (State)
        {
            case CreatureState.Idle:
                GetDirInput();
                GetIdleInput();
                break;
            case CreatureState.Moving:
                break;
        }
    }

    private void LateUpdate()
    {
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }

    void GetDirInput()  // 키 입력 시 상태 지정
    {
        if (Input.GetKey(KeyCode.W))
        {
            Dir = MoveDir.Up;   
        }
        else if (Input.GetKey(KeyCode.S))
        {
            Dir = MoveDir.Down;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            Dir = MoveDir.Left;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            Dir = MoveDir.Right;
        }
        else
        {
            Dir = MoveDir.None;
        }
    }

    void GetIdleInput()
    {
        if (Input.GetKey(KeyCode.Space))
            State = CreatureState.Skill;
    }
}
