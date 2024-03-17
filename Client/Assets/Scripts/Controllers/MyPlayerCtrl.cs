using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MyPlayerCtrl : PlayerCtrl
{
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
                break;
            case CreatureState.Moving:
                GetDirInput();
                break;
        }
    }

    protected override void UpdateIdle()
    {
        #region Moving direction
        if (Dir != MoveDir.None)
        {
            State = CreatureState.Moving;
            return;
        }
        #endregion

        #region Skills
        switch (currentSkill)
        {
            case Skills.Punch:
                if (Input.GetKey(KeyCode.Space))
                {
                    State = CreatureState.Skill;
                    _coSkill = StartCoroutine("coPunchSkill");
                }
                break;
            case Skills.ArrowShot:
                if (Input.GetKey(KeyCode.Space))
                {
                    State = CreatureState.Skill;
                    _coSkill = StartCoroutine("coArrowSkill");
                }
                break;
        }

        if (Input.GetKeyDown(KeyCode.Q))        // 스킬 모드 전환 주먹 - 화살 - ...
        {
            if (currentSkill > Skills.Punch)
                currentSkill -= 1;
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentSkill < Skills.ArrowShot)
                currentSkill += 1;
        }
        #endregion
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
}
