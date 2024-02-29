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

        GameObject player = Managers.resourceMgr.Instantiate("Creature/Player");
        player.name = "Player";
        Managers.objectMgr.Add(player);

        for (int i = 0; i < 5; i++)
        {
            GameObject monster = Managers.resourceMgr.Instantiate("Creature/Monster");
            monster.name = $"Monster{i + 1}";
            MonsterCtrl mc = monster.GetComponent<MonsterCtrl>();

            // 랜덤 위치 스폰
            while (true)
            {                
                Vector3Int pos = new Vector3Int() { x = Random.Range(-15, 15), y = Random.Range(-5, 5) };
                //Debug.Log(pos);
                //Debug.Log(Managers.mapMgr.CanGo(pos));
                if (Managers.mapMgr.CanGo(pos) && Managers.objectMgr.SearchPos(pos) == null) // 생성위치에 Collision없고 오브젝트도 없으면 생성
                {
                    mc.CellPos = pos;
                    Managers.objectMgr.Add(monster);
                    break;
                }
            }
        }
    }


    public override void Clear()
    {

    }
}
