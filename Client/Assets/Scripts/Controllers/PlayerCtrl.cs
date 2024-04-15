using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Define;
using Google.Protobuf.Protocol;

public class PlayerCtrl : CreatureCtrl
{
    //GameObject (Player) ����
    protected bool CanReceiveInput = true;

    protected Coroutine _coSkill;
    [SerializeField] protected Define.Skills currentSkill = Skills.Punch;

    protected float PunchCoolTime = 0.3f;   // �ִϸ��̼� ���̶� ����
    protected float ArrowCoolTime = 0.2f;

    protected override void Init()
    {
        yoffset = 0.7f;
        base.Init();
        AddHpBar();
    }

    protected override void UpdateAnim()    // Skills�� ���� �ִϸ��̼� ������ ���� override
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
                    _spriteRenderer.flipX = true;  // SpriteRenderer�� flipX������ ��������Ʈ �¿����
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
                            _spriteRenderer.flipX = true;  // SpriteRenderer�� flipX������ ��������Ʈ �¿����
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
        // �ǰ� ������ ���������� �̵�
        currentSkill = Skills.Punch;

        // ��� �ð�
        State = CreatureState.Skill;
        yield return new WaitForSeconds(PunchCoolTime);  // ���� �� Ŭ���̾�Ʈ������ ��Ÿ���� üũ�Ͽ� ������ �δ��� �ٿ���
        State = CreatureState.Idle;
        _coSkill = null;
        CheckUpdatedFlag(); // State = Idle�� �Ǿ��ٴ� ���� ������ �˷����ϴµ� ���� ��Ŷ���� �ı� ���ٴ� ������ ��� �ӽÿ� Todo
    }

    IEnumerator coArrowSkill()
    {
        // �ǰ� ���� ���������� �̵�
        currentSkill = Skills.ArrowShot;

        // ��� �ð�
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
