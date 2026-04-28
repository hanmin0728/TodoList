using System;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : Singleton<UpgradeManager>
{
    public Dictionary<string, UpgradeData> UpgradeDictionary { get; private set; } = new Dictionary<string, UpgradeData>();
    public bool IsInitialized { get; private set; } = false;

    public event Action OnUpgradeDataLoaded;

    private void Start()
    {
        // CSVManager가 이미 로딩을 끝냈다면 바로 파싱 시작
        if (CSVManager.Instance.IsInitialized)
        {
            LoadUpgradeData();
        }
        else
        {
            // 아직 로딩 중이라면 로딩 완료 이벤트에 대기 등록
            CSVManager.Instance.OnLoadingComplete += LoadUpgradeData;
        }

    }

    /// <summary>
    /// CSV에서 데이터를 읽어와 딕셔너리에 넣어주는 함수
    /// </summary>
    private void LoadUpgradeData()
    {
        // 이벤트 중복 호출 방지
        CSVManager.Instance.OnLoadingComplete -= LoadUpgradeData;

        UpgradeDictionary.Clear();

        var table = CSVManager.Instance.GetTable("UpgradeTable");

        if (table == null)
        {
            Debug.LogError("UpgradeTable을 찾을 수 없습니다.");
            return;
        }

        foreach (var row in table)
        {
            UpgradeData data = ParseRowToData(row);
            if (!UpgradeDictionary.ContainsKey(data.ID))
            {
                UpgradeDictionary.Add(data.ID, data);
            }
        }

        IsInitialized = true;
        OnUpgradeDataLoaded?.Invoke();

        Debug.Log($"[UpgradeManager] {UpgradeDictionary.Count}개의 데이터 로드 완료.");
    }

    //Dictionary를 UpgradeData로 변환
    private UpgradeData ParseRowToData(Dictionary<string, object> row)
    {
        string rawValue = "0";
        if (row.ContainsKey("IsPercentageStat"))
        {
            rawValue = row["IsPercentageStat"].ToString();
        }

        return new UpgradeData
        {
            ID = row["ID"].ToString(),
            Name = row["Name"].ToString(),
            BaseValue = float.Parse(row["BaseValue"].ToString()),
            IncreasePerLevel = float.Parse(row["IncreasePerLevel"].ToString()),
            BaseCost = double.Parse(row["BaseCost"].ToString()),
            CostMultiplier = float.Parse(row["CostMultiplier"].ToString()),
            MaxLevel = int.Parse(row["MaxLevel"].ToString()),
            IsPercentageStat = (rawValue == "1")
        };
    }

    /// <summary>
    /// 능력치 데이터를 꺼내주는 함수 
    /// </summary>
    public UpgradeData GetUpgradeData(string id)
    {
        if (UpgradeDictionary.TryGetValue(id, out UpgradeData data))
        {
            return data;
        }

        Debug.LogError($"[UpgradeManager] '{id}' ID에 해당하는 업그레이드 데이터를 찾을 수 없습니다!");
        return null;
    }
}
