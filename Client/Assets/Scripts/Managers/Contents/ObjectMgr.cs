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

    public GameObject SearchPos(Vector3Int cellPos)
    {
        foreach (GameObject go in _objects)
        {
            CreatureCtrl cc = go.GetComponent<CreatureCtrl>();
            if (cc == null) // 없어도 걍 null return때리고 crash안나게 continue
                continue;

            if (cc.CellPos == cellPos)
                return go;
        }
        return null;
    }

    public void Clear()
    {
        _objects.Clear();
    }

}
