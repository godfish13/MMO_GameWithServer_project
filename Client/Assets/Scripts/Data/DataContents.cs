using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Stat
// public or [SerializeField] �����ؾ��� JSON���� ������ �޾ƿ� �� ����
// �� �׸��� �̸��̶� JSON ���� �� �׸��� �̸��� �� ���ƾ� ������ �޾ƿ� �� ����
// �ڷ��� ���� ����!
[Serializable]      // �޸𸮿� ����ִ� ������ ���Ϸ� ��ȯ��Ű�� ���� �ʿ��� ���� // �׳� ���...
public class Stat
{
    public int level;       
    public int maxHp;
    public int totalExp;       
    public int attack;      
}

[Serializable]
public class StatData : ILoader<int, Stat>
{
    public List<Stat> Stats = new List<Stat>();     // !!!!!!�߿�!!!!!! JSON���Ͽ��� �޾ƿ����� list�� �̸��� ��!!! ���ƾ���

    public Dictionary<int, Stat> MakeDict()
    {
        Dictionary<int, Stat> dict = new Dictionary<int, Stat>();
        foreach (Stat stat in Stats)
            dict.Add(stat.level, stat);
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