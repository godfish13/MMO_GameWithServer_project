using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Utils
{
    public static T GetOrAddComponent<T>(GameObject Target) where T : UnityEngine.Component // Target�� T�� �������� �ҷ����� �ƴϸ� ����
    {
        T Component = Target.GetComponent<T>();
        if (Component == null)
            Component = Target.AddComponent<T>();
        return Component;
    }

    public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)   // UI�׸� ���� GameObject�� enum�� �����ϱ� ����
    {                                                                                               // �Ϲ�ȭ ���� GameObject������ ������
        Transform transform =  FindChild<Transform>(go, name, recursive);      // GameObject�� Monobehavior, ... �� ��ӹ��� ���� ���̶� ����
        if (transform == null)                                                 // �����϶�� ������ �׷��Ƿ� �Ϲ�ȭ�ƴѹ��� ����
            return null;

        return transform.gameObject;
    }

    public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
    {                                                               //recursive : child�׸� �Ʒ��� child���� Ž������ üũ
        if (go == null)
            return null;

        if(recursive == false)
        {
            for(int i = 0; i < go.transform.childCount; i++)
            {
                Transform transform = go.transform.GetChild(i);
                if(string.IsNullOrEmpty(name) || transform.name == name)
                {
                    T component = transform.GetComponent<T>();
                    if(component != null)
                        return component;
                }
            }
        }
        else
        {
            foreach (T component in go.GetComponentsInChildren<T>())
            {
                if (string.IsNullOrEmpty(name) || component.name == name)
                    return component;
            }
        }

        return null;
    }
}
