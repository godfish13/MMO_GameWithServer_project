using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class PlayerCtrl : CreatureCtrl
{
    //GameObject (Player) 부착
    private bool CanReceiveInput = true;

    Coroutine _coSkill;
    public Define.Skills currentSkill = Skills.Punch;

    protected override void Init()
    {
        yoffset = 0.7f;
        base.Init();
    }

    protected override void UpdateAnim()    // Skills에 따른 애니메이션 변경을 위해 override
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
                    _spriteRenderer.flipX = true;  // SpriteRenderer의 flipX값으로 스프라이트 좌우반전
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
                            _spriteRenderer.flipX = true;  // SpriteRenderer의 flipX값으로 스프라이트 좌우반전
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
                            _spriteRenderer.flipX = true;  // SpriteRenderer의 flipX값으로 스프라이트 좌우반전
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
                break;
            case CreatureState.Moving:
                GetDirInput();
                break;
        }
    }

    private void LateUpdate()
    {
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
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

    IEnumerator coPunchSkill()
    {
        // 피격 판정
        GameObject go = Managers.objectMgr.SearchPos(GetFrontCellPos());
        if (go != null)
        {
            Debug.Log(go.name);

            CreatureCtrl cc = go.GetComponent<CreatureCtrl>();
            if (cc != null)
            {
                cc.OnDamaged();
            }
        }

        // 대기 시간
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

        // 피격판정은 ArrowCtrl에서 관리

        // 대기 시간
        currentSkill = Skills.ArrowShot;
        yield return new WaitForSeconds(0.3f);
        State = CreatureState.Idle;
        _coSkill = null;
    }
}
