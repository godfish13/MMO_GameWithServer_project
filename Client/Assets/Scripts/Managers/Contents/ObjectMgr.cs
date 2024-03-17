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

    public void Add(PlayerInfo info, bool MyCtrl = false) // MyCtrl : ���� �����ϴ��� �ƴ��� üũ
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

    public GameObject SearchPos(Vector3Int cellPos) // �Է��� Cell position�� cc������Ʈ�� ������ �ش� ������Ʈ�� return
    {
        foreach (GameObject obj in _objects.Values)
        {
            CreatureCtrl cc = obj.GetComponent<CreatureCtrl>();
            if (cc == null) // ��� �� null return������ crash�ȳ��� continue
                continue;

            if (cc.CellPos == cellPos)
                return obj;
        }
        return null;
    }

    public GameObject FindGameObject(Func<GameObject, bool> condition)  // go�� �Ű�������, bool�� return�ϴ� ���ٽ�
    {                                                                    
        foreach (GameObject obj in _objects.Values)
        {
            CreatureCtrl cc = obj.GetComponent<CreatureCtrl>();
            if (cc == null)
                continue;

            if (condition.Invoke(obj))      // ���ٽ� condition�� true�� ������ obj�� ������ �ش� obj return
                return obj;
        }
        return null;
    }

    public void Clear()
    {
        _objects.Clear();
    }
}
