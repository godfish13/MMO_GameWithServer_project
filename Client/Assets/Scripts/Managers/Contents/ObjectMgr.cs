using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMgr
{
    //Dictionary<int, GameObject> _objects = new Dictionary<int, GameObject>();  // todo
    List<GameObject> _objects = new List<GameObject>();

    public void Add(GameObject go)
    {
        _objects.Add(go);
    }

    public void Remove(GameObject go) 
    {
        _objects.Remove(go);
    }

    public GameObject SearchPos(Vector3Int cellPos) // �Է��� Cell position�� cc������Ʈ�� ������ �ش� ������Ʈ�� return
    {
        foreach (GameObject obj in _objects)
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
        foreach (GameObject obj in _objects)
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
