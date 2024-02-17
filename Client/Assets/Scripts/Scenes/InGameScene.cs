using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameScene : BaseScene
{
    protected override void init()
    {
        base.init();
        SceneType = Define.Scene.InGame;
        Managers.mapMgr.LoadMap(1);
    }


    public override void Clear()
    {

    }
}
