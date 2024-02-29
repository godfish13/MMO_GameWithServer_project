using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MonsterCtrl : CreatureCtrl
{
    // Monster·ù gameobject ºÎÂø

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
}
