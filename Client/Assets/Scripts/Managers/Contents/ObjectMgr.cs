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

    public void Add(PlayerInfo info, bool myCtrl = false) // MyCtrl : ���� �����ϴ��� �ƴ��� üũ
    {
        if (myCtrl == true)
        {
            GameObject go = Managers.resourceMgr.Instantiate("Creature/MyPlayer");
            go.name = info.Name;
            _objects.Add(info.PlayerId, go);

            myPlayerCtrl = go.GetComponent<MyPlayerCtrl>();
            myPlayerCtrl.Id = info.PlayerId;
            myPlayerCtrl.PosInfo = info.PosInfo;
            myPlayerCtrl.SyncPos();     // ������ ��ġ�� ����Ƽ�� ��ġ ����ȭ
        }
        else
        {
            GameObject go = Managers.resourceMgr.Instantiate("Creature/Player");
            go.name = info.Name;
            _objects.Add(info.PlayerId, go);

            PlayerCtrl pc = go.GetComponent<PlayerCtrl>();
            pc.Id = info.PlayerId;
            pc.PosInfo = info.PosInfo;
            pc.SyncPos();        // ������ ��ġ�� ����Ƽ�� ��ġ ����ȭ
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
