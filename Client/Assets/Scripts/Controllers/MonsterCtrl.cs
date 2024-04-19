using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf.Protocol;

public class MonsterCtrl : CreatureCtrl
{
    // Monster�� gameobject ����
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
        // �ϴ� �Ѵ���� �ٷ� ��� ���� todo
        //Managers.objectMgr.Remove(Id);
        //Managers.resourceMgr.Destroy(gameObject);
    }

    IEnumerator coPunchSkill()
    {
        // �ǰ� ����
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

        // ��� �ð�
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

        // �ǰ������� ArrowCtrl���� ����

        // ��� �ð�
        yield return new WaitForSeconds(0.4f);
        State = CreatureState.Moving;
        _coSkill = null;
    }
}
