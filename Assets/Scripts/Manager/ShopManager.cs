using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
public class ShopRateData
{
    public int Level;
    public int RequireCount;
    public float NormalRate;
    public float RareRate;
    public float EpicRate;
    public float LegendRate;
}

public class ShopManager : Singleton<ShopManager>
{
    [Header("장비 티어 등장 확률 (4T, 3T, 2T, 1T)")]
    [SerializeField] private float[] tierRates = new float[] { 65f, 20f, 10f, 5f };

    private Dictionary<int, ShopRateData> shopRateDic = new Dictionary<int, ShopRateData>();
    public bool IsInitialized { get; private set; } = false;

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
    }
    private void LoadShopRateData()
    {
        CSVManager.Instance.OnLoadingComplete -= LoadShopRateData;
        shopRateDic.Clear();

        var table = CSVManager.Instance.GetTable("ShopTable");

        if (table == null)
        {
            Debug.LogError("ShopTable 찾을 수 없습니다! CSVManager의 FileNames를 확인하세요.");
            return;
        }

        foreach (var row in table)
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

                if (!shopRateDic.ContainsKey(data.Level))
                {
                    shopRateDic.Add(data.Level, data);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"상점 데이터 파싱 중 오류 발생! Level: {row["ShopLevel"]} / 에러: {e.Message}");
            }
        }
        IsInitialized = true;
        Debug.Log($"<color=cyan>[ShopManager]</color> 상점 확률 테이블 {shopRateDic.Count}개 로드 완료!");
    }

    /// <summary>
    /// 통합 소환 시스템 (무기, 반지, 스킬 공용)
    /// </summary>
    public List<string> SummonItems(string category, int count)
    {
        List<string> results = new List<string>();

        if (!IsInitialized)
        {
            Debug.LogWarning("[ShopManager] 아직 데이터가 로드되지 않았습니다!");
            return results;
        }

        for (int i = 0; i < count; i++)
        {
            int currentLevel = SaveManager.Instance.CurrentData.GetShopLevel();

            if (!shopRateDic.ContainsKey(currentLevel))
            {
                Debug.LogError($"[ShopManager] 레벨 {currentLevel}의 데이터가 없습니다.");
                break;
            }

            ShopRateData currentRate = shopRateDic[currentLevel];

            // 경험치 증가
            SaveManager.Instance.CurrentData.AddShopSummonCount(1);
            int currentExp = SaveManager.Instance.CurrentData.GetShopSummonCount();

            // 레벨업 체크
            if (currentExp >= currentRate.RequireCount && shopRateDic.ContainsKey(currentLevel + 1))
            {
                currentLevel++;
                SaveManager.Instance.CurrentData.SetShopLevel(currentLevel);
                currentRate = shopRateDic[currentLevel];
                Debug.Log($"상점 레벨업! -> Lv.{currentLevel}");
            }

            // 확률 굴리기
            GradeType rolledGrade = RollGrade(currentRate);
            string itemID = string.Empty;

            if (category == "Weapon" || category == "Ring")
            {
                itemID = GetRandomEquipment(category, rolledGrade);
            }
            else if (category == "Skill")
            {
                //스킬 뽑는거 나중에 추가
            }

            if (!string.IsNullOrEmpty(itemID))
            {
                results.Add(itemID);
                ApplySummonResult(category, itemID);

                Debug.Log($"[소환 진행 중] 카테고리: {category} | 등급: <color=orange>{rolledGrade}</color> | 획득 ID: {itemID}");
            }
        }

        SaveManager.Instance.SaveGame();

        if (category == "Weapon" || category == "Ring")
        {
            EquipmentManager.OnEquipmentDataChanged?.Invoke();
        }

        string resultSummary = string.Join(", ", results);
        Debug.Log($"<color=yellow>[최종 소환 결과]</color> {category} {count}회 뽑기 완료!\n획득 목록: {resultSummary}");

        return results;
    }

    private GradeType RollGrade(ShopRateData rate)
    {
        float randomVal = Random.Range(0f, 100f);
        float cumulative = 0f;

        cumulative += rate.NormalRate;
        if (randomVal <= cumulative) return GradeType.Normal;

        cumulative += rate.RareRate;
        if (randomVal <= cumulative) return GradeType.Rare;

        cumulative += rate.EpicRate;
        if (randomVal <= cumulative) return GradeType.Epic;

        return GradeType.Legend;
    }

    private string GetRandomEquipment(string category, GradeType grade)
    {
        float randomVal = Random.Range(0f, 100f);
        float cumulative = 0f;
        int selectedTier = 1;

        for (int i = 0; i < tierRates.Length; i++)
        {
            cumulative += tierRates[i];
            if (randomVal <= cumulative)
            {
                selectedTier = i + 1;
                break;
            }
        }

        Enum.TryParse(category, out EquipmentType typeEnum);

        var items = EquipmentManager.Instance.EquipDataDic.Values.Where(x => x.EquipType == typeEnum && x.Grade == grade && x.Tier == selectedTier)
            .ToList();

        if (items.Count == 0) return string.Empty;

        return items[Random.Range(0, items.Count)].ID;
    }

    private void ApplySummonResult(string category, string id)
    {
        var data = SaveManager.Instance.CurrentData;

        if (category == "Weapon" || category == "Ring")
        {
            if (data.GetEquipCount(id) == 0)
            {
                data.SetNewStatus(id, true);
            }
            data.AddEquipCount(id, 1);
        }
    }
}
