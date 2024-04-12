using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Stat
// public or [SerializeField] �����ؾ��� JSON���� ������ �޾ƿ� �� ����
// �� �׸��� �̸��̶� JSON ���� �� �׸��� �̸��� �� ���ƾ� ������ �޾ƿ� �� ����
// �ڷ��� ���� ����!
[Serializable]
public class StatData : ILoader<int, StatInfo>
{
    public List<StatInfo> stats = new List<StatInfo>();     // !!!!!!�߿�!!!!!! JSON���Ͽ��� �޾ƿ����� list�� �̸��� ��!!! ���ƾ���

    public Dictionary<int, StatInfo> MakeDict()
    {
        Dictionary<int, StatInfo> dict = new Dictionary<int, StatInfo>();
        foreach (StatInfo stat in stats)
        {
            stat.Hp = stat.MaxHp;
            dict.Add(stat.Level, stat);
        }

        return dict;
    }
}
#endregion

#region Skill
[Serializable]
public class Skill
{
    public int id;
    public string name;
    public float cooldown;
    public int damage;
    public SkillType skillType;
    public ProjectileInfo projectile;
}

public class ProjectileInfo
{
    public string name;
    public float speed;
    public int range;
    public string prefab;
}

public class SkillData : ILoader<int, Skill>
{
    public List<Skill> skills = new List<Skill>();     // !!!!!!�߿�!!!!!! JSON���Ͽ��� �޾ƿ����� list�� �̸��� ��!!! ���ƾ���

    public Dictionary<int, Skill> MakeDict()
    {
        Dictionary<int, Skill> dict = new Dictionary<int, Skill>();
        foreach (Skill skill in skills)
            dict.Add(skill.id, skill);
        return dict;
    }
}
#endregion