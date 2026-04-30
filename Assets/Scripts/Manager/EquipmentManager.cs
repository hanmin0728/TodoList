using System;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : Singleton<EquipmentManager>
{
    public Dictionary<string, EquipmentData> EquipDataDic = new Dictionary<string, EquipmentData>();

    public event Action OnDataInitialized; 
    public bool IsInitialized { get; private set; } = false;
    private Dictionary<string, Sprite> iconCache = new Dictionary<string, Sprite>();

    public EquipmentGradeData gradeData;

    public static Action OnEquipmentDataChanged;

    private void Start()
    {
        if (CSVManager.Instance.IsInitialized)
        {
            LoadEquipmentData();
        }
        else
        {
            CSVManager.Instance.OnLoadingComplete += LoadEquipmentData;
        }
    }
    private void LoadEquipmentData()
    {
        CSVManager.Instance.OnLoadingComplete -= LoadEquipmentData;
        EquipDataDic.Clear();

        var table = CSVManager.Instance.GetTable("EquipmentTable");

        if (table == null)
        {
            Debug.LogError("EquipmentTable을 찾을 수 없습니다! CSVManager의 FileNames를 확인하세요.");
            return;
        }

        foreach (var row in table)
        {
            try
            {
                EquipmentData data = new EquipmentData
                {
                    ID = row["ID"].ToString(),
                    Name = row["Name"].ToString(),

                    EquipType = (EquipmentType)Enum.Parse(typeof(EquipmentType), row["EquipType"].ToString()),
                    Grade = (GradeType)Enum.Parse(typeof(GradeType), row["Grade"].ToString()),

                    Tier = int.Parse(row["Tier"].ToString()),

                    // 장착시 올라가는 스탯 1
                    EquipStatType_1 = (StatType)Enum.Parse(typeof(StatType), row["EquipStatType_1"].ToString()),
                    EquipStatValue_1 = double.Parse(row["EquipStatValue_1"].ToString()),

                    // 장착시 올라가는 스탯 2
                    EquipStatType_2 = (StatType)Enum.Parse(typeof(StatType), row["EquipStatType_2"].ToString()),
                    EquipStatValue_2 = double.Parse(row["EquipStatValue_2"].ToString()),

                    // 보유 효과
                    OwnStatType = (StatType)Enum.Parse(typeof(StatType), row["OwnStatType"].ToString()),
                    OwnStatValue = double.Parse(row["OwnStatValue"].ToString()),

                    // 합성 및 리소스
                    NextID = row["NextID"].ToString(),
                    NeedCount = int.Parse(row["NeedCount"].ToString()),
                    IconName = row["IconName"].ToString()
                };

                if (!EquipDataDic.ContainsKey(data.ID))
                {
                    EquipDataDic.Add(data.ID, data);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"데이터 파싱 중 오류 발생! ID: {row["ID"]} / 에러: {e.Message}");
            }
        }
        IsInitialized = true;
        
        OnDataInitialized?.Invoke();

        Debug.Log($"장비 데이터 {EquipDataDic.Count}개 로드 완료!");
    }

    public Sprite GetIcon(EquipmentData data)
    {
        if (data == null || string.IsNullOrEmpty(data.IconName)) return null;

        if (iconCache.TryGetValue(data.IconName, out Sprite cachedSprite))
        {
            return cachedSprite;
        }

        string folderName = data.EquipType.ToString();
        string fullPath = $"Icons/Equipment/{folderName}/{data.IconName}";

        Sprite sp = Resources.Load<Sprite>(fullPath);

        if (sp != null)
        {
            iconCache.Add(data.IconName, sp);
        }
        else
        {
            Debug.LogWarning($"아이콘 로드 실패: {fullPath}");
        }

        return sp;
    }

    /// <summary>
    /// 장비 합성 
    /// </summary>
    /// <param name="currentID">합성할 현재 장비 ID</param>
    /// <returns>합성 성공 여부</returns>
    public bool Synthesize(string currentID)
    {
        if (!EquipDataDic.TryGetValue(currentID, out var currentData))
        {
            Debug.LogError($"합성 오류: ID {currentID}를 찾을 수 없습니다.");
            return false;
        }

        // 다음 등급 장비가 있는지 확인
        string nextID = currentData.NextID;
        if (string.IsNullOrEmpty(nextID) || nextID == "Max")
        {
            Debug.LogWarning("최고 등급 장비이거나 다음 장비 데이터가 없습니다.");
            return false;
        }

        int initialOwnedCount = SaveManager.Instance.CurrentData.GetEquipCount(currentID);
        int needCount = currentData.NeedCount;

        if (initialOwnedCount < needCount)
        {
            Debug.LogWarning($"재료 부족: {initialOwnedCount} / {needCount}");
            return false;
        }
        
        int currentOwnedCount = initialOwnedCount;
        int synthesesPerformed = 0;

        while (currentOwnedCount >= needCount)
        {
            currentOwnedCount -= needCount;
            SaveManager.Instance.CurrentData.SetEquipCount(currentID, currentOwnedCount);

            if (SaveManager.Instance.CurrentData.GetEquipCount(nextID) == 0)
            {
                SaveManager.Instance.CurrentData.SetNewStatus(nextID, true);
            }

            SaveManager.Instance.CurrentData.AddEquipCount(nextID, 1);

            synthesesPerformed++;
        }

        if (synthesesPerformed > 0)
        {
            SaveManager.Instance.SaveGame();

            OnEquipmentDataChanged?.Invoke();

            Debug.Log($"{currentData.Name} {synthesesPerformed * needCount}개 소모 -> {nextID} 장비 {synthesesPerformed}개 획득 성공!");
        }
        return true;
    }

    public bool CanSynthesizeAny()
    {
        foreach (var data in EquipDataDic.Values)
        {
            if (string.IsNullOrEmpty(data.NextID) || data.NextID == "Max") continue;

            // 하나라도 필요 개수보다 많이 가지고 있다면 true
            if (SaveManager.Instance.CurrentData.GetEquipCount(data.ID) >= data.NeedCount)
            {
                return true;
            }
        }
        return false;
    }

    public void SynthesizeAll()
    {
        bool isAnySynthesized = false;

        // 모든 장비 데이터를 돌면서 체크
        foreach (var data in EquipDataDic.Values)
        {
            // 5개 이상일 때만 반복해서 합성 (재료가 부족할 때까지)
            while (SaveManager.Instance.CurrentData.GetEquipCount(data.ID) >= data.NeedCount)
            {
                if (string.IsNullOrEmpty(data.NextID) || data.NextID == "Max") break;

                // 로직 실행 (재료 차감 -> 결과 추가)
                int currentCount = SaveManager.Instance.CurrentData.GetEquipCount(data.ID);
                string nextID = data.NextID;
                SaveManager.Instance.CurrentData.SetEquipCount(data.ID, currentCount - data.NeedCount);

                if (SaveManager.Instance.CurrentData.GetEquipCount(nextID) == 0)
                {
                    SaveManager.Instance.CurrentData.SetNewStatus(nextID, true);
                }

                SaveManager.Instance.CurrentData.AddEquipCount(data.NextID, 1);

                isAnySynthesized = true;
            }
        }

        if (isAnySynthesized)
        {
            SaveManager.Instance.SaveGame();
            OnEquipmentDataChanged?.Invoke(); 
        }
    }
}
