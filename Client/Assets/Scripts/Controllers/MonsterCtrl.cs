using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf.Protocol;

public class MonsterCtrl : CreatureCtrl
{
    // Monster류 gameobject 부착
    Coroutine _coSkill;
    [SerializeField] bool _isRange = false;

    public bool isRange { get; set; } = false;

    protected override void Init()
    {
        base.Init();

        State = CreatureState.Idle;
        Dir = MoveDir.Down;

        _isRange = (Random.Range(0, 2) == 0 ? true : false);
    }

    protected override void UpdateIdle()
    {
        base.UpdateIdle();
    }

    public override void OnDamaged()
    {
        // 일단 한대맞음 바로 사망 수정 todo
        //Managers.objectMgr.Remove(Id);
        //Managers.resourceMgr.Destroy(gameObject);
    }

    IEnumerator coPunchSkill()
    {
        // 피격 판정
        GameObject go = Managers.objectMgr.FindCreatureInCellPos(GetFrontCellPos());
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
        yield return new WaitForSeconds(0.5f);
        State = CreatureState.Moving;
        _coSkill = null;
    }

    IEnumerator coRockSkill()
    {
        GameObject go = Managers.resourceMgr.Instantiate("Creature/Rock");
        ArrowCtrl ac = go.GetComponent<ArrowCtrl>();
        ac.Dir = Dir;
        ac.CellPos = CellPos;

        // 피격판정은 ArrowCtrl에서 관리

        // 대기 시간
        yield return new WaitForSeconds(0.4f);
        State = CreatureState.Moving;
        _coSkill = null;
    }
}
