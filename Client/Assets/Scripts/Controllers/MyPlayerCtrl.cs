using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;
using Google.Protobuf.Protocol;

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
                    Debug.Log("Skill used");
                    C_Skill skillPacket = new C_Skill() { SkillInfo = new SkillInfo() };
                    skillPacket.SkillInfo.SkillId = 1;
                    Managers.networkMgr.Send(skillPacket);  

                    _coSkillCoolTimer = StartCoroutine("CoInputCoolTimer", PunchCoolTime);   // ��ų ��� ��Ŷ ��û ��Ÿ�� 0.3��
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

        if (Input.GetKeyDown(KeyCode.Q))        // ��ų ��� ��ȯ �ָ� - ȭ�� - ...
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

    Coroutine _coSkillCoolTimer;
    IEnumerator CoInputCoolTimer(float time)
    {
        yield return new WaitForSeconds(time);
        _coSkillCoolTimer = null;
    }

    private void LateUpdate()
    {
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }

    void GetDirInput()  // Ű �Է� �� ���� ����
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

    protected override void CalculateDestPos()   // C_Move Packet �����ϰ� ������
    {
        if (Dir == MoveDir.None)
        {
            State = CreatureState.Idle;
            CheckUpdatedFlag();         // Idle state�� ���ϴ��� C_Move pkt ���� �� �ֵ��� ���⿡ ���������� �߰�
            return;
        }

        Vector3Int destPos = CellPos;

        switch (Dir)
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

        if (Managers.mapMgr.CanGo(destPos))     // �̵� ������ ��ǥ���� üũ �� �̵�
        {
            if (Managers.objectMgr.SearchPos(destPos) == null)
            {
                CellPos = destPos;
            }
        }

        CheckUpdatedFlag();     // _updated �÷��׿� ���� C_Move �߼�
    }

    protected override void CheckUpdatedFlag()
    {
        if (_updated == true)
        {
            C_Move movePacket = new C_Move();
            movePacket.PosInfo = base.PosInfo;  // ���ü� base
            Managers.networkMgr.Send(movePacket);
            _updated = false;
        }       
    }
}
