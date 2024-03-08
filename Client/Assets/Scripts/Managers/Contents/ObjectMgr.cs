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

    public GameObject SearchPos(Vector3Int cellPos) // 입력한 Cell position에 cc오브젝트가 있으면 해당 오브젝트를 return
    {
        foreach (GameObject obj in _objects)
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
        foreach (GameObject obj in _objects)
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
