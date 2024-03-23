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

    public void Add(PlayerInfo info, bool myCtrl = false) // MyCtrl : 내가 조종하는지 아닌지 체크
    {
        if (myCtrl == true)
        {
            GameObject go = Managers.resourceMgr.Instantiate("Creature/MyPlayer");
            go.name = info.Name;
            _objects.Add(info.PlayerId, go);

            myPlayerCtrl = go.GetComponent<MyPlayerCtrl>();
            myPlayerCtrl.Id = info.PlayerId;
            myPlayerCtrl.PosInfo = info.PosInfo;
            myPlayerCtrl.SyncPos();     // 서버상 위치와 유니티상 위치 동기화
        }
        else
        {
            GameObject go = Managers.resourceMgr.Instantiate("Creature/Player");
            go.name = info.Name;
            _objects.Add(info.PlayerId, go);

            PlayerCtrl pc = go.GetComponent<PlayerCtrl>();
            pc.Id = info.PlayerId;
            pc.PosInfo = info.PosInfo;
            pc.SyncPos();        // 서버상 위치와 유니티상 위치 동기화
        }
    }

    public void RemoveMyPlayer()
    {
        if (myPlayerCtrl = null)
            return;

        Remove(myPlayerCtrl.Id);
        myPlayerCtrl = null;
    }

    public void Remove(int Id) 
    {
        GameObject go = FindGameObjectbyId(Id);
        if (go == null)
            return;
        
        _objects.Remove(Id);
        Managers.resourceMgr.Destroy(go);
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

    public GameObject FindGameObjectbyId(int id)
    {
        GameObject go = null;
        _objects.TryGetValue(id, out go);
        return go;
    }

    public void Clear()
    {
        foreach (GameObject obj in _objects.Values)
        {
            Managers.resourceMgr.Destroy(obj);
        }
        _objects.Clear();
    }
}
