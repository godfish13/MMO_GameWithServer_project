using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MonsterCtrl : CreatureCtrl
{
    // Monster�� gameobject ����

    protected override void Init()
    {
        base.Init();

        State = CreatureState.Idle;
        Dir = MoveDir.None;
    }

    protected override void UpdateCtrl()
    {
        base.UpdateCtrl();
    }

    public override void OnDamaged()
    {
        GameObject de = Managers.resourceMgr.Instantiate("Effect/DeathBoom");   // ����Ʈ ���
        de.transform.position = transform.position;
        de.GetComponent<Animator>().Play("DeathBoom");
        Managers.resourceMgr.Destroy(de, 0.4f);

        // ��� �̤�
        Managers.objectMgr.Remove(gameObject);
        Managers.resourceMgr.Destroy(gameObject);
    }
}
