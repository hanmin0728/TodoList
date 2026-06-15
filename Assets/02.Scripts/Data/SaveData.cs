using System;
using UnityEngine;

[Serializable]
public sealed class SaveData
{
    [SerializeField] private double gold;
    [SerializeField] private double gem;

    [SerializeField] private int currentStageID = 101;
    [SerializeField] private int currentWaveIndex = 1;

    [SerializeField] private SerializationDictionary<string, int> UpgradeLevels = new SerializationDictionary<string, int>();

    // Key: 장비 ID, Value: 보유 개수
    [SerializeField] private SerializationDictionary<string, int> OwnedEquipments = new SerializationDictionary<string, int>(); 
    //Key: 장비 타입(Weapon, Ring 등), Value: 장착 중인 장비 ID
    [SerializeField] private SerializationDictionary<string, string> EquippedEquipments = new SerializationDictionary<string, string>(); 
    [SerializeField] private SerializationDictionary<string, bool> NewItemStatus = new SerializationDictionary<string, bool>(); //처음 얻는 장비인지 확인

    [SerializeField] private int shopLevel = 1;
    [SerializeField] private int shopSummonCount;

    public SaveData()
    {
        InitDefaultData();
    }

    #region 재화
    public double GetGem() => gem;
    public void SetGem(double amount) => gem = Math.Max(0d, amount);

    public double GetGold() => gold;
    public void SetGold(double amount) => gold = Math.Max(0d, amount);
    #endregion

    #region 스테이지
    public int GetStageID() => currentStageID;
    public int GetWaveIndex() => currentWaveIndex;

    public void SetStageProgress(int stageID, int waveIndex)
    {
        if (stageID < 101 || waveIndex < 1)
        {
            return;
        }

        currentStageID = stageID;
        currentWaveIndex = waveIndex;
    }
    #endregion

    #region 장비
    public int GetUpgradeLevel(string id)
    {
        return UpgradeLevels.TryGetValue(id, out int level) ? level : 0;
    }

    public void SetUpgradeLevel(string id, int level)
    {
        UpgradeLevels[id] = Math.Max(0, level);
    }

    public bool IsNewItem(string id)
    {
        return NewItemStatus.TryGetValue(id, out bool isNew) && isNew;
    }

    public bool IsUnlocked(string id)
    {
        return NewItemStatus.ContainsKey(id);
    }

    public void SetNewStatus(string id, bool isNew)
    {
        NewItemStatus[id] = isNew;
    }

    public int GetEquipCount(string id)
    {
        return OwnedEquipments.TryGetValue(id, out int count) ? count : 0;
    }

    public void AddEquipCount(string id, int count)
    {
        SetEquipCount(id, GetEquipCount(id) + count);
    }

    public void SetEquipCount(string id, int count)
    {
        OwnedEquipments[id] = Math.Max(0, count);
    }

    public string GetEquippedID(string type)
    {
        return EquippedEquipments.TryGetValue(type, out string id) ? id : string.Empty;
    }

    public void SetEquippedID(string type, string id)
    {
        EquippedEquipments[type] = id ?? string.Empty;
    }
    #endregion

    #region 상점

    public int GetShopLevel() => shopLevel;
    public void SetShopLevel(int level) => shopLevel = Math.Max(1, level);

    public int GetShopSummonCount() => shopSummonCount;
    public void AddShopSummonCount(int count) => shopSummonCount = Math.Max(0, shopSummonCount + count);
    #endregion

    public void InitDefaultData()
    {
        if (UpgradeLevels.Count > 0)
        {
            return;
        }

        //초기값 설정

        UpgradeLevels["Atk"] = 1;
        UpgradeLevels["Hp"] = 1;
        UpgradeLevels["AtkSpeed"] = 1;
        UpgradeLevels["CriticalChance"] = 1;
        UpgradeLevels["CriticalDamage"] = 1;

        gold = 100;
        currentStageID = 101;
        currentWaveIndex = 1;

        OwnedEquipments["Weapon_1"] = 1;
        NewItemStatus["Weapon_1"] = true;

        EquippedEquipments["Weapon"] = string.Empty;
        EquippedEquipments["Ring"] = string.Empty;

        shopLevel = 1;
        shopSummonCount = 0;
    }
}
