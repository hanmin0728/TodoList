using System;
using UnityEngine;

public enum EquipmentType
{
    Weapon,
    Ring
}

public enum GradeType
{
    Normal,
    Rare,
    Epic,
    Legend
}

public enum StatType
{
    None,
    Atk,
    Hp,
    AtkSpeed,
    CriticalChance,
    CriticalDamage
}

[Serializable]
public sealed class EquipmentData
{
    public string ID;
    public string Name;
    public EquipmentType EquipType;
    public GradeType Grade;
    public int Tier;

    [Header("장착시 증가 Stat 1")] 
    public StatType EquipStatType_1;
    public double EquipStatValue_1;

    [Header("장착시 증가 Stat 2")] 
    public StatType EquipStatType_2;
    public double EquipStatValue_2;

    [Header("보유시 증가 Stat")]
    public StatType OwnStatType;
    public double OwnStatValue;

    [Header("합성")]
    public string NextID;
    public int NeedCount;

    [Header("Resource")]
    public string IconName;
}
