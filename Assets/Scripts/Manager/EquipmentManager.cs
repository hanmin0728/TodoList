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

    private void Start()
    {
        if (CSVManager.Instance.IsInitialized)
        {
            InitEquipmentData();
        }
        else
        {
            CSVManager.Instance.OnLoadingComplete += InitEquipmentData;
        }
    }
    private void InitEquipmentData()
    {
        CSVManager.Instance.OnLoadingComplete -= InitEquipmentData;
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
    public EquipmentData GetEquipData(string id)
    {
        if (EquipDataDic.TryGetValue(id, out var data)) return data;
        return null;
    }

    public Sprite GetIcon(EquipmentData data)
    {
        if (data == null || string.IsNullOrEmpty(data.IconName)) return null;

        if (iconCache.TryGetValue(data.IconName, out Sprite cachedSprite))
        {
            Debug.Log("캐싱된거 잘 쓰고있음");
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
}
