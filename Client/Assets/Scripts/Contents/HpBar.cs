using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpBar : MonoBehaviour
{
    [SerializeField]
    Transform _hpBar = null;

    public void SetHpBar(float ratio)
    {
        ratio = Mathf.Clamp01(ratio);   // == Mathf.Clamp(ratio, 0, 1) : ratio�� 0, 1 ���̰����θ� return���� �����ִ�!  // �̰ɷ� ��ų��ü��Ʈ ��ü�ص��ɵ�
        _hpBar.localScale = new Vector3(ratio, 1, 1);      
    }
}
