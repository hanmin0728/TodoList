using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class UpgradeManager : Singleton<UpgradeManager>
{
    // 데이터 변경 시 UI를 갱신하기 위한 이벤트 (ID를 넘겨서 해당 셀만 갱신 가능하게 함)
    public event Action<string> OnUpgradeChanged;

    private readonly Dictionary<string, UpgradeData> upgradeDataById = new Dictionary<string, UpgradeData>();

    public bool IsInitialized { get; private set; }
    public event Action OnUpgradeDataLoaded;

    private void Start()
    {
        if (CSVManager.Instance.IsInitialized)
        {
            LoadUpgradeData();
        }
        else
        {
            CSVManager.Instance.OnLoadingComplete += LoadUpgradeData;
        }
    }
    public bool TryUpgrade(string id)
    {
        UpgradeData data = GetUpgradeData(id);

        if (data == null) 
            return false;

        int currentLevel = SaveManager.Instance.CurrentData.GetUpgradeLevel(id);
        if (currentLevel >= data.MaxLevel) 
            return false;

        double cost = data.GetCost(currentLevel);
        if (!CurrencyManager.Instance.TrySpendGold(cost))
            return false;

        SaveManager.Instance.CurrentData.SetUpgradeLevel(id, currentLevel + 1);

        SaveManager.Instance.SaveToServerAsync().Forget();
        SaveManager.Instance.SaveGameSync();

        OnUpgradeChanged?.Invoke(id);

        return true;

    }
    public UpgradeData GetUpgradeData(string id)
    {
        if (upgradeDataById.TryGetValue(id, out UpgradeData data))
        {
            return data;
        }

        Debug.LogError($"[UpgradeManager] Upgrade data is missing. ID: {id}");
        return null;
    }

    public IEnumerable<UpgradeData> GetAllUpgradeData()
    {
        return upgradeDataById.Values;
    }

    private void LoadUpgradeData()
    {
        CSVManager.Instance.OnLoadingComplete -= LoadUpgradeData;
        upgradeDataById.Clear();

        List<Dictionary<string, object>> table = CSVManager.Instance.GetTable("UpgradeTable");
        if (table == null)
        {
            Debug.LogError("[UpgradeManager] UpgradeTable is missing. Check CSVManager file names.");
            return;
        }

        foreach (Dictionary<string, object> row in table)
        {
            try
            {
                UpgradeData data = ParseRowToData(row);
                if (!upgradeDataById.ContainsKey(data.ID))
                {
                    upgradeDataById.Add(data.ID, data);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[UpgradeManager] Failed to parse upgrade data. ID: {row["ID"]}, Error: {e.Message}");
            }
        }

        IsInitialized = true;
        OnUpgradeDataLoaded?.Invoke();

        Debug.Log($"[UpgradeManager] Upgrade table loaded. Count: {upgradeDataById.Count}");
    }

    private static UpgradeData ParseRowToData(Dictionary<string, object> row)
    {
        string percentageStatValue = row.ContainsKey("IsPercentageStat")
            ? row["IsPercentageStat"].ToString()
            : "0";

        return new UpgradeData
        {
            ID = row["ID"].ToString(),
            Name = row["Name"].ToString(),
            BaseValue = float.Parse(row["BaseValue"].ToString()),
            IncreasePerLevel = float.Parse(row["IncreasePerLevel"].ToString()),
            BaseCost = double.Parse(row["BaseCost"].ToString()),
            CostMultiplier = float.Parse(row["CostMultiplier"].ToString()),
            MaxLevel = int.Parse(row["MaxLevel"].ToString()),
            IsPercentageStat = percentageStatValue == "1"
        };
    }
}
