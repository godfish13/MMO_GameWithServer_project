using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpBar : MonoBehaviour
{
    [SerializeField]
    Transform _hpBar = null;

    public void SetHpBar(float ratio)
    {
        ratio = Mathf.Clamp01(ratio);   // == Mathf.Clamp(ratio, 0, 1) : ratio가 0, 1 사이값으로만 return해줌 쓸모있다!  // 이걸로 스킬교체파트 교체해도될듯
        _hpBar.localScale = new Vector3(ratio, 1, 1);      
    }
}
