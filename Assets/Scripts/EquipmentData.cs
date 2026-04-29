using UnityEngine;
using System;
public enum EquipmentType { Weapon, Ring }
public enum GradeType { Normal, Rare, Epic, Legend }
public enum StatType { None, Atk, Hp, AtkSpeed, CriticalChance, CriticalDamage}

[Serializable]
public class EquipmentData 
{
    public string ID;                // 장비 고유 ID 
    public string Name;              // 장비 이름 (설명)
    public EquipmentType EquipType;  // 장비 종류 (Weapon, Ring)
    public GradeType Grade;          // 등급
    public int Tier;                 // 티어 (1~4)

    [Header("장착 능력치 1")]
    public StatType EquipStatType_1;
    public double EquipStatValue_1;

    [Header("장착 능력치 2")]
    public StatType EquipStatType_2;
    public double EquipStatValue_2;
    
    [Header("보유 능력치")]
    public StatType OwnStatType;
    public double OwnStatValue;

    [Header("합성 관련")]
    public string NextID;            // 합성 시 다음 아이템 ID 
    public int NeedCount;            // 합성 필요 개수

    [Header("리소스")]
    public string IconName;          // 아이콘 이미지 파일명
}
