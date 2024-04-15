using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Define;
using Google.Protobuf.Protocol;

public class PlayerCtrl : CreatureCtrl
{
    //GameObject (Player) 부착
    protected bool CanReceiveInput = true;

    protected Coroutine _coSkill;
    [SerializeField] protected Define.Skills currentSkill = Skills.Punch;

    protected float PunchCoolTime = 0.3f;   // 애니메이션 길이랑 통일
    protected float ArrowCoolTime = 0.2f;

    protected override void Init()
    {
        yoffset = 0.7f;
        base.Init();
        AddHpBar();
    }

    protected override void UpdateAnim()    // Skills에 따른 애니메이션 변경을 위해 override
    {
        if (_animator == null || _spriteRenderer == null)
            return;

        if (State == CreatureState.Idle)
        {
            switch (Dir)
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
            switch (Dir)
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
                    switch (Dir)
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
                    switch (Dir)
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
    }


    public void useSkill(int skillId)
    {
        if (skillId == 1)
        {
            _coSkill = StartCoroutine("coPunchSkill");
        }
        else if (skillId == 2)
        {
            _coSkill = StartCoroutine("coArrowSkill");
        }
    }

    protected virtual void CheckUpdatedFlag()
    {

    }

    IEnumerator coPunchSkill()
    {
        // 피격 판정은 서버단으로 이동
        currentSkill = Skills.Punch;

        // 대기 시간
        State = CreatureState.Skill;
        yield return new WaitForSeconds(PunchCoolTime);  // 서버 외 클라이언트에서도 쿨타임을 체크하여 서버의 부담을 줄여줌
        State = CreatureState.Idle;
        _coSkill = null;
        CheckUpdatedFlag(); // State = Idle이 되었다는 것을 서버에 알려야하는데 따로 패킷까지 파긴 오바니 쓰던거 잠깐 임시용 Todo
    }

    IEnumerator coArrowSkill()
    {
        // 피격 판정 서버단으로 이동
        currentSkill = Skills.ArrowShot;

        // 대기 시간
        State = CreatureState.Skill;
        yield return new WaitForSeconds(ArrowCoolTime);
        State = CreatureState.Idle;
        _coSkill = null;
        CheckUpdatedFlag();
    }

    public override void OnDamaged()
    {
        Debug.Log("Player Hitted!");
    }
}
