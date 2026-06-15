using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public sealed class ShopRateData
{
    public int Level;
    public int RequireCount;
    public float NormalRate;
    public float RareRate;
    public float EpicRate;
    public float LegendRate;
}

public sealed class ShopManager : Singleton<ShopManager>
{
    [SerializeField] private float[] tierRates = { 65f, 20f, 10f, 5f };

    private readonly Dictionary<int, ShopRateData> shopRateByLevel = new Dictionary<int, ShopRateData>();
    private readonly Dictionary<EquipmentPoolKey, List<string>> equipmentIdsByPoolKey = new Dictionary<EquipmentPoolKey, List<string>>();

    public bool IsInitialized { get; private set; }

    private readonly struct EquipmentPoolKey : IEquatable<EquipmentPoolKey>
    {
        private readonly EquipmentType equipmentType;
        private readonly GradeType grade;
        private readonly int tier;

        public EquipmentPoolKey(EquipmentType equipmentType, GradeType grade, int tier)
        {
            this.equipmentType = equipmentType;
            this.grade = grade;
            this.tier = tier;
        }

        public bool Equals(EquipmentPoolKey other)
        {
            return equipmentType == other.equipmentType && grade == other.grade && tier == other.tier;
        }

        public override bool Equals(object obj)
        {
            return obj is EquipmentPoolKey other && Equals(other);
        }
        public override int GetHashCode() => HashCode.Combine((int)equipmentType, (int)grade, tier); // 🌟 C# 8.0의 빠르고 안전한 해시 조합기 사용
      
    }

    private void Start()
    {
        if (CSVManager.Instance.IsInitialized)
        {
            LoadShopRateData();
        }
        else
        {
            CSVManager.Instance.OnLoadingComplete += LoadShopRateData;
        }

        if (EquipmentManager.Instance.IsInitialized)
        {
            RebuildEquipmentIdPool();
        }
        else
        {
            EquipmentManager.Instance.OnDataInitialized += RebuildEquipmentIdPool;
        }
    }

    private void LoadShopRateData()
    {
        CSVManager.Instance.OnLoadingComplete -= LoadShopRateData;
        shopRateByLevel.Clear();

        List<Dictionary<string, object>> table = CSVManager.Instance.GetTable("ShopTable");
        if (table == null)
        {
            Debug.LogError("[ShopManager] ShopTable is missing. Check CSVManager file names.");
            return;
        }

        foreach (Dictionary<string, object> row in table)
        {
            try
            {
                ShopRateData data = new ShopRateData
                {
                    Level = int.Parse(row["ShopLevel"].ToString()),
                    RequireCount = int.Parse(row["RequireCount"].ToString()),
                    NormalRate = float.Parse(row["Normal_Rate"].ToString()),
                    RareRate = float.Parse(row["Rare_Rate"].ToString()),
                    EpicRate = float.Parse(row["Epic_Rate"].ToString()),
                    LegendRate = float.Parse(row["Legend_Rate"].ToString())
                };

                if (!shopRateByLevel.ContainsKey(data.Level))
                {
                    shopRateByLevel.Add(data.Level, data);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[ShopManager] Failed to parse shop data. Level: {row["ShopLevel"]}, Error: {e.Message}");
            }
        }

        IsInitialized = true;
        Debug.Log($"[ShopManager] Shop rate table loaded. Count: {shopRateByLevel.Count}");
    }

    public bool CanSummonEquipment()
    {
        return IsInitialized && EnsureEquipmentPoolReady();
    }

    public List<string> SummonItems(EquipmentType equipmentType, int count)
    {
        List<string> results = new List<string>(count);

        if (!IsInitialized)
        {
            Debug.LogWarning("[ShopManager] Shop data is not loaded yet.");
            return results;
        }

        if (!EnsureEquipmentPoolReady())
        {
            Debug.LogWarning("[ShopManager] Equipment data is not ready yet.");
            return results;
        }

        SaveData saveData = SaveManager.Instance.CurrentData;

        for (int i = 0; i < count; i++)
        {
            int currentLevel = saveData.GetShopLevel();
            if (!shopRateByLevel.TryGetValue(currentLevel, out ShopRateData currentRate))
            {
                Debug.LogError($"[ShopManager] Shop level data is missing. Level: {currentLevel}");
                break;
            }

            saveData.AddShopSummonCount(1);
            int summonCount = saveData.GetShopSummonCount();

            if (summonCount >= currentRate.RequireCount &&
                shopRateByLevel.TryGetValue(currentLevel + 1, out ShopRateData nextRate))
            {
                currentLevel++;
                saveData.SetShopLevel(currentLevel);
                currentRate = nextRate;
                Debug.Log($"[ShopManager] Shop level up. Level: {currentLevel}");
            }

            GradeType rolledGrade = RollGrade(currentRate);
            string itemId = GetRandomEquipmentId(equipmentType, rolledGrade);

            if (string.IsNullOrEmpty(itemId))
            {
                continue;
            }

            results.Add(itemId);
            ApplySummonResult(saveData, itemId);
        }

        SaveManager.Instance.SaveGameSync();
        EquipmentManager.NotifyEquipmentDataChanged();
        return results;
    }

    private GradeType RollGrade(ShopRateData rate)
    {
        float randomValue = Random.Range(0f, 100f);
        float cumulativeRate = rate.NormalRate;

        if (randomValue <= cumulativeRate)
        {
            return GradeType.Normal;
        }

        cumulativeRate += rate.RareRate;
        if (randomValue <= cumulativeRate)
        {
            return GradeType.Rare;
        }

        cumulativeRate += rate.EpicRate;
        if (randomValue <= cumulativeRate)
        {
            return GradeType.Epic;
        }

        return GradeType.Legend;
    }

    private string GetRandomEquipmentId(EquipmentType equipmentType, GradeType grade)
    {
        int selectedTier = RollTier();
        EquipmentPoolKey key = new EquipmentPoolKey(equipmentType, grade, selectedTier);

        if (!equipmentIdsByPoolKey.TryGetValue(key, out List<string> itemIds) || itemIds.Count == 0)
        {
            return string.Empty;
        }

        return itemIds[Random.Range(0, itemIds.Count)];
    }

    private int RollTier()
    {
        float randomValue = Random.Range(0f, 100f);
        float cumulativeRate = 0f;

        for (int i = 0; i < tierRates.Length; i++)
        {
            cumulativeRate += tierRates[i];
            if (randomValue <= cumulativeRate)
            {
                return i + 1;
            }
        }

        return tierRates.Length;
    }

    private static void ApplySummonResult(SaveData saveData, string itemId)
    {
        if (saveData.GetEquipCount(itemId) == 0)
        {
            saveData.SetNewStatus(itemId, true);
        }

        saveData.AddEquipCount(itemId, 1);
    }

    private bool EnsureEquipmentPoolReady()
    {
        if (equipmentIdsByPoolKey.Count > 0)
        {
            return true;
        }

        if (!EquipmentManager.Instance.IsInitialized)
        {
            return false;
        }

        RebuildEquipmentIdPool();
        return equipmentIdsByPoolKey.Count > 0;
    }

    private void RebuildEquipmentIdPool()
    {
        EquipmentManager.Instance.OnDataInitialized -= RebuildEquipmentIdPool;
        equipmentIdsByPoolKey.Clear();

        foreach (EquipmentData data in EquipmentManager.Instance.GetAllEquipmentData())
        {
            EquipmentPoolKey key = new EquipmentPoolKey(data.EquipType, data.Grade, data.Tier);

            if (!equipmentIdsByPoolKey.TryGetValue(key, out List<string> itemIds))
            {
                itemIds = new List<string>();
                equipmentIdsByPoolKey.Add(key, itemIds);
            }

            itemIds.Add(data.ID);
        }
    }
}
