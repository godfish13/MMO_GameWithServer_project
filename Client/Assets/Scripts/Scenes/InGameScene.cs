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

        /*co = StartCoroutine("CoExplode", 4.0f);     // �̷�������ε� ��밡��
        StopCoroutine(CoStopExplode(2.0f));*/

        Dictionary<int, Stat> dict = Managers.dataMgr.StatDictionary;

        //gameObject.GetOrAddComponent<CursorCtrl>();   //Ŀ�� �̹��� �߰��ؼ� ����ϱ�
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
