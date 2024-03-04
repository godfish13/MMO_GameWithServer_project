using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class PlayerCtrl : CreatureCtrl
{
    //GameObject (Player) ����

    Coroutine _coSkill;
    public Define.Skills currentSkill = Skills.Punch;

    protected override void Init()
    {
        base.Init();
    }

    protected override void UpdateAnim()    // Skills�� ���� �ִϸ��̼� ������ ���� override
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
                    _spriteRenderer.flipX = true;  // SpriteRenderer�� flipX������ ��������Ʈ �¿����
                    break;
            }
        }
        else if (State == CreatureState.Skill)
        {
            switch (currentSkill)
            {
                case Skills.Punch:
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
                            _spriteRenderer.flipX = true;  // SpriteRenderer�� flipX������ ��������Ʈ �¿����
                            break;
                    }
                    break;
                case Skills.ArrowShot:
                    switch (_lastDir)
                    {
                        case MoveDir.Up:
                            _animator.Play("ATTACK_WEAPON_BACK");
                            _spriteRenderer.flipX = false;
                            break;
                        case MoveDir.Down:
                            _animator.Play("ATTACK_WEAPON_FRONT");
                            _spriteRenderer.flipX = false;
                            break;
                        case MoveDir.Right:
                            _animator.Play("ATTACK_WEAPON_RIGHT");
                            _spriteRenderer.flipX = false;
                            break;
                        case MoveDir.Left:
                            _animator.Play("ATTACK_WEAPON_RIGHT");
                            _spriteRenderer.flipX = true;  // SpriteRenderer�� flipX������ ��������Ʈ �¿����
                            break;
                    }
                    break;
            }          
        }
        else
        {
            //Todo
        }
    }

    protected override void UpdateCtrl()
    {        
        base.UpdateCtrl();

        switch (State)
        {
            case CreatureState.Idle:
                GetDirInput();
                GetIdleInput();
                ChangeSkillInput();
                break;
            case CreatureState.Moving:
                break;
        }
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

    void GetIdleInput()
    {
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
    }

    void ChangeSkillInput()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (currentSkill > Skills.Punch)
                currentSkill -= 1;        
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentSkill < Skills.ArrowShot)
                currentSkill += 1;
        }
    }

    IEnumerator coPunchSkill()
    {
        // �ǰ� ����
        GameObject go = Managers.objectMgr.SearchPos(GetFrontCellPos());
        if (go != null)
        {
            Debug.Log(go.name);
        }

        // ��� �ð�
        currentSkill = Skills.Punch;
        yield return new WaitForSeconds(0.3f);
        State = CreatureState.Idle;
        _coSkill = null;
    }

    IEnumerator coArrowSkill()
    {       
        GameObject go = Managers.resourceMgr.Instantiate("Creature/Arrow");
        ArrowCtrl ac = go.GetComponent<ArrowCtrl>();
        ac.Dir = _lastDir;
        ac.CellPos = CellPos;

        // �ǰ������� ArrowCtrl���� ����

        // ��� �ð�
        currentSkill = Skills.ArrowShot;
        yield return new WaitForSeconds(0.3f);
        State = CreatureState.Idle;
        _coSkill = null;
    }
}
