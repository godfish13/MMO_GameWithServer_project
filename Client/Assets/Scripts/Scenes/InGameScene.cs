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

        Screen.SetResolution(640, 480, false); // ȭ�� ũ�� ���� (����ũ��, ����ũ��, ��üȭ�鿩��)

        /*GameObject player = Managers.resourceMgr.Instantiate("Creature/Player");
        player.name = "Player";
        Managers.objectMgr.Add(player);

        for (int i = 0; i < 5; i++)
        {
            GameObject monster = null;
            if (Random.Range(0,2) == 0 ? true : false)
            {
                monster = Managers.resourceMgr.Instantiate("Creature/Monster");
                monster.name = $"Monster{i + 1}";
                monster.GetComponent<MonsterCtrl>().isRange = false;
            }
            else
            {
                monster = Managers.resourceMgr.Instantiate("Creature/Monster_Range");
                monster.name = $"Monster{i + 1}";
                monster.GetComponent<MonsterCtrl>().isRange = true;
            }
            MonsterCtrl mc = monster.GetComponent<MonsterCtrl>();

            // ���� ��ġ ����
            while (true)
            {                
                Vector3Int pos = new Vector3Int() { x = Random.Range(-15, 15), y = Random.Range(-5, 5) };
                //Debug.Log(pos);
                //Debug.Log(Managers.mapMgr.CanGo(pos));
                if (Managers.mapMgr.CanGo(pos) && Managers.objectMgr.SearchPos(pos) == null) // ������ġ�� Collision���� ������Ʈ�� ������ ����
                {
                    mc.CellPos = pos;
                    Managers.objectMgr.Add(monster);
                    break;
                }
            }
        }*/
    }


    public override void Clear()
    {

    }
}
