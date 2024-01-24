using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameScene : BaseScene
{
    Coroutine co;

    protected override void init()
    {
        base.init();
        SceneType = Define.Scene.InGame;

        Managers.UIMgr.ShowSceneUI<UI_Inven>();
        Managers.UIMgr.ShowPopUpUI<UI_Btn>();   

        /*co = StartCoroutine("CoExplode", 4.0f);     // 이런모양으로도 사용가능
        StopCoroutine(CoStopExplode(2.0f));*/

        Dictionary<int, Stat> dict = Managers.dataMgr.StatDictionary;

        //gameObject.GetOrAddComponent<CursorCtrl>();   //커서 이미지 추가해서 사용하기
    }

    IEnumerator CoStopExplode(float seconds)
    {
        Debug.Log("Stop Enter...");
        yield return new WaitForSeconds(seconds);
        Debug.Log("Exlode Stopped");
        if (co != null)
        {
            StopCoroutine(co);
            co = null;
        }
    }

    IEnumerator CoExplode(float seconds)
    {
        Debug.Log("Explode Enter...");
        yield return new WaitForSeconds(seconds);
        Debug.Log("KABOOM!");
    }

    public override void Clear()
    {

    }

}
