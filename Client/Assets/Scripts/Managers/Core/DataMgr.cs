using Google.Protobuf.Protocol;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface ILoader<key, value>            // DataContents���� Ȱ��
{
    Dictionary<key, value> MakeDict();
}

public class DataMgr
{
    public Dictionary<int, StatInfo> StatDictionary { get; private set; } = new Dictionary<int, StatInfo>();
    public Dictionary<int, Skill> SkillDictionary { get; private set; } = new Dictionary<int, Skill>();

    public void init()
    {
        StatDictionary = LoadJson<StatData, int, StatInfo>("StatData").MakeDict();
        SkillDictionary = LoadJson<SkillData, int, Skill>("SkillData").MakeDict();
    }

    Loader LoadJson<Loader, key, value>(string path) where Loader : ILoader<key, value>
    {
        TextAsset textAsset = Managers.resourceMgr.Load<TextAsset>($"Data/{path}");
        return JsonUtility.FromJson<Loader>(textAsset.text);
    }
}