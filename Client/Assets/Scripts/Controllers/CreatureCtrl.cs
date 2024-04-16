using Google.Protobuf.Protocol;
using UnityEngine;

public class CreatureCtrl : BaseCtrl
{
    // Player, Monster, etc ... ctrl base class

    [SerializeField] HpBar _hpBar;

    public override StatInfo Stat
    {
        get { return base.Stat; }
        set
        {
            if (base.Stat.Equals(value))
                return;

            base.Stat = value;
            UpdateHpBar();
        }
    }

    public override int Hp
    {
        get { return Stat.Hp; }
        set 
        { 
            base.Hp = value;
            UpdateHpBar();
        }
    }

    protected void AddHpBar()
    {
        GameObject go = Managers.resourceMgr.Instantiate("UI/HpBar", transform);
        go.transform.localPosition = new Vector3(0, 0.5f, 0);
        go.name = "HpBar";
        _hpBar = go.GetComponent<HpBar>();
        UpdateHpBar();
    }

    void UpdateHpBar()
    {
        if (_hpBar == null)
        {
            //Debug.Log("Cant find HpBar!");
            return;
        }
          
        float ratio = 0.0f;
        if (Stat.MaxHp > 0) // �����°��� 0 ���ϸ� ũ�����̹Ƿ� ����üũ
            ratio = (float)Hp / Stat.MaxHp;

        _hpBar.SetHpBar(ratio);
    }

    protected override void Init()
    {
        base.Init();
        AddHpBar();
    }

    public virtual void OnDamaged()
    {
        
    }

    public virtual void OnDead()
    {
        State = CreatureState.Dead;

        // ��� anim ���� todo

        //�ǰ� ����Ʈ ���
        GameObject de = Managers.resourceMgr.Instantiate("Effect/DeathBoom");   // ����Ʈ ���
        de.transform.position = transform.position;
        de.GetComponent<Animator>().Play("DeathBoom");
        Managers.resourceMgr.Destroy(de, 0.4f);
    }
}
