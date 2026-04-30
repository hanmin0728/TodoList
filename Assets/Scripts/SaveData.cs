using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    #region 재화
    [SerializeField] private double gold = 0;
    public double GetGold() => gold;
    public void SetGold(double cost)
    {
        gold = cost;
    }
    #endregion

    #region stage 관련
    [SerializeField] private int currentStageID = 101;
    [SerializeField] private int currentWaveIndex = 1;

    public int GetStageID() => currentStageID;
    public int GetWaveIndex() => currentWaveIndex;
    public void SetStageProgress(int stageID, int waveIndex)
    {
        if (stageID < 101 || waveIndex < 1) return;

        currentStageID = stageID;
        currentWaveIndex = waveIndex;
    }
    #endregion

    #region Upgrade Data
    // Key: ID, Value: Level
    [SerializeField] private SerializationDictionary<string, int> UpgradeLevels = new SerializationDictionary<string, int>();

    public int GetUpgradeLevel(string id) => UpgradeLevels.ContainsKey(id) ? UpgradeLevels[id] : 0;
    public void SetUpgradeLevel(string id, int level) => UpgradeLevels[id] = level;
    #endregion

    #region Equipment Data
    // Key: 장비 ID, Value: 보유 개수
    [SerializeField] private SerializationDictionary<string, int> OwnedEquipments = new SerializationDictionary<string, int>();

    // Key: 장비 타입(Weapon, Ring 등), Value: 장착 중인 장비 ID
    [SerializeField] private SerializationDictionary<string, string> EquippedEquipments = new SerializationDictionary<string, string>();

    public int GetEquipCount(string id) => OwnedEquipments.ContainsKey(id) ? OwnedEquipments[id] : 0;
    public void AddEquipCount(string id, int count)
    {
        OwnedEquipments[id] = GetEquipCount(id) + count;
    }
    public void SetEquipCount(string id, int count) => OwnedEquipments[id] = count;

    public string GetEquippedID(string type) => EquippedEquipments.ContainsKey(type) ? EquippedEquipments[type] : string.Empty;

    public void SetEquippedID(string type, string id) => EquippedEquipments[type] = id;

    #endregion

    public SaveData()
    {
        InitDefaultData();
    }
    public void InitDefaultData()
    {
        // 딕셔너리에 데이터가 없을 때만 기본값 세팅 (중복 방지)
        if (UpgradeLevels.ToDictionary().Count == 0)
        {
            //csv 파일 id랑 같아야함
            UpgradeLevels["Atk"] = 1;      // 공격력
            UpgradeLevels["Hp"] = 1;          // 체력
            UpgradeLevels["AtkSpeed"] = 1;    // 공격속도
            UpgradeLevels["CriticalChance"] = 1; // 치명타확률
            UpgradeLevels["CriticalDamage"] = 1;   // 치명타데미지

            gold = 100;

            // 스테이지 기본 데이터 초기화
            currentStageID = 101;
            currentWaveIndex = 1;

            // 장비 기본 데이터 초기화
            OwnedEquipments["Weapon_1"] = 1;

            // 기본 장착 설정
            EquippedEquipments["Weapon"] = string.Empty;
            EquippedEquipments["Ring"] = string.Empty;

        }
    }

}


