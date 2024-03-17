using Google.Protobuf;
using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMgr
{
    public MyPlayerCtrl myPlayerCtrl { get; set; }
    Dictionary<int, GameObject> _objects = new Dictionary<int, GameObject>();
    // playerId, player

    public void Add(PlayerInfo info, bool MyCtrl = false) // MyCtrl : 내가 조종하는지 아닌지 체크
    {
        if (MyCtrl == true)
        {
            GameObject go = Managers.resourceMgr.Instantiate("Creature/MyPlayer");
            go.name = info.Name;
            _objects.Add(info.PlayerId, go);

            myPlayerCtrl = go.GetComponent<MyPlayerCtrl>();
            myPlayerCtrl.Id = info.PlayerId;
            myPlayerCtrl.CellPos = new Vector3Int(info.PosX, info.PosY, 0);
        }
        else
        {
            GameObject go = Managers.resourceMgr.Instantiate("Creature/Player");
            go.name = info.Name;
            _objects.Add(info.PlayerId, go);

            PlayerCtrl pc = go.GetComponent<PlayerCtrl>();
            pc.Id = info.PlayerId;
            pc.CellPos = new Vector3Int(info.PosX, info.PosY, 0);
        }
    }

    public void RemoveMyPlayer()
    {
        if (myPlayerCtrl = null)
            return;

        Remove(myPlayerCtrl.Id);
        myPlayerCtrl = null;
    }

    public void Add(int Id, GameObject go)
    {
        _objects.Add(Id, go);
    }

    public void Remove(int Id) 
    {
        _objects.Remove(Id);
    }

    public GameObject SearchPos(Vector3Int cellPos) // 입력한 Cell position에 cc오브젝트가 있으면 해당 오브젝트를 return
    {
        foreach (GameObject obj in _objects.Values)
        {
            CreatureCtrl cc = obj.GetComponent<CreatureCtrl>();
            if (cc == null) // 없어도 걍 null return때리고 crash안나게 continue
                continue;

            if (cc.CellPos == cellPos)
                return obj;
        }
        return null;
    }

    public GameObject FindGameObject(Func<GameObject, bool> condition)  // go를 매개변수로, bool을 return하는 람다식
    {                                                                    
        foreach (GameObject obj in _objects.Values)
        {
            CreatureCtrl cc = obj.GetComponent<CreatureCtrl>();
            if (cc == null)
                continue;

            if (condition.Invoke(obj))      // 람다식 condition이 true를 리턴한 obj가 있으면 해당 obj return
                return obj;
        }
        return null;
    }

    public void Clear()
    {
        _objects.Clear();
    }
}
