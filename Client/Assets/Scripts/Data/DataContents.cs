using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Stat
// public or [SerializeField] 선언해야지 JSON에서 데이터 받아올 수 있음
// 각 항목의 이름이랑 JSON 파일 내 항목의 이름이 꼭 같아야 데이터 받아올 수 있음
// 자료형 또한 주의!
[Serializable]
public class StatData : ILoader<int, StatInfo>
{
    public List<StatInfo> stats = new List<StatInfo>();     // !!!!!!중요!!!!!! JSON파일에서 받아오려는 list와 이름이 꼭!!! 같아야함

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
    public List<Skill> skills = new List<Skill>();     // !!!!!!중요!!!!!! JSON파일에서 받아오려는 list와 이름이 꼭!!! 같아야함

    public Dictionary<int, Skill> MakeDict()
    {
        Dictionary<int, Skill> dict = new Dictionary<int, Skill>();
        foreach (Skill skill in skills)
            dict.Add(skill.id, skill);
        return dict;
    }
}
#endregion