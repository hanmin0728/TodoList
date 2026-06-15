using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class EquipmentManager : Singleton<EquipmentManager>
{
    [SerializeField] private EquipmentGradeData gradeData;

    private readonly Dictionary<string, EquipmentData> equipmentDataById = new Dictionary<string, EquipmentData>();
    private readonly Dictionary<string, Sprite> iconCacheByPath = new Dictionary<string, Sprite>();

    public static event Action EquipmentDataChanged;

    public event Action OnDataInitialized;

    public bool IsInitialized { get; private set; }
    public EquipmentGradeData GradeData => gradeData;


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

    public static void NotifyEquipmentDataChanged()
    {
        EquipmentDataChanged?.Invoke();
    }

    public bool TryGetEquipmentData(string equipmentId, out EquipmentData data)
    {
        return equipmentDataById.TryGetValue(equipmentId, out data);
    }

    public IEnumerable<EquipmentData> GetAllEquipmentData()
    {
        return equipmentDataById.Values;
    }

    public Sprite GetIcon(EquipmentData data)
    {
        if (data == null || string.IsNullOrEmpty(data.IconName))
        {
            return null;
        }

        string resourcePath = $"Icons/Equipment/{data.EquipType}/{data.IconName}";
        if (iconCacheByPath.TryGetValue(resourcePath, out Sprite cachedSprite))
        {
            return cachedSprite;
        }

        Sprite sprite = Resources.Load<Sprite>(resourcePath);
        if (sprite == null)
        {
            Debug.LogWarning($"[EquipmentManager] Failed to load icon: Resources/{resourcePath}");
            return null;
        }

        iconCacheByPath.Add(resourcePath, sprite);
        return sprite;
    }

    public bool Synthesize(string currentId)
    {
        if (!equipmentDataById.TryGetValue(currentId, out EquipmentData currentData) || !CanSynthesize(currentData))
            return false;

        SaveData saveData = SaveManager.Instance.CurrentData;
        int ownedCount = saveData.GetEquipCount(currentId);
        int synthesizeCount = ownedCount / currentData.NeedCount;

        if (synthesizeCount <= 0) return false;

        saveData.SetEquipCount(currentId, ownedCount % currentData.NeedCount);

        if (saveData.GetEquipCount(currentData.NextID) == 0)
            saveData.SetNewStatus(currentData.NextID, true);

        saveData.AddEquipCount(currentData.NextID, synthesizeCount);

        SaveManager.Instance.SaveGameSync();
        NotifyEquipmentDataChanged();
        return true;
    }

    public bool CanSynthesizeAny()
    {
        SaveData saveData = SaveManager.Instance.CurrentData;

        foreach (EquipmentData data in equipmentDataById.Values)
        {
            if (CanSynthesize(data) && saveData.GetEquipCount(data.ID) >= data.NeedCount)
            {
                return true;
            }
        }

        return false;
    }

    public void SynthesizeAll()
    {
        SaveData saveData = SaveManager.Instance.CurrentData;
        bool hasAnySynthesized = false;
        bool isSynthesizing = true;    

        while (isSynthesizing)
        {
            isSynthesizing = false;

            foreach (EquipmentData data in equipmentDataById.Values)
            {
                if (!CanSynthesize(data)) continue;

                int ownedCount = saveData.GetEquipCount(data.ID);
                int synthesizeCount = ownedCount / data.NeedCount;

                if (synthesizeCount > 0)
                {
                    saveData.SetEquipCount(data.ID, ownedCount % data.NeedCount);

                    if (saveData.GetEquipCount(data.NextID) == 0)
                        saveData.SetNewStatus(data.NextID, true);

                    saveData.AddEquipCount(data.NextID, synthesizeCount);

                    isSynthesizing = true; 
                    hasAnySynthesized = true;
                }
            }
        }

        if (hasAnySynthesized)
        {
            SaveManager.Instance.SaveGameSync();
            NotifyEquipmentDataChanged();
        }
    }

    public void EquipItem(string equipmentId)
    {
        if (!equipmentDataById.TryGetValue(equipmentId, out EquipmentData data))
        {
            Debug.LogError($"[EquipmentManager] Cannot equip missing equipment. ID: {equipmentId}");
            return;
        }

        SaveData saveData = SaveManager.Instance.CurrentData;
        saveData.SetEquippedID(data.EquipType.ToString(), equipmentId);

        SaveManager.Instance.SaveGameSync();
        NotifyEquipmentDataChanged();

        Debug.Log($"[EquipmentManager] Equipped {data.Name}. Type: {data.EquipType}, ID: {equipmentId}");
    }

    public void MarkItemAsSeen(string equipmentId)
    {
        SaveData saveData = SaveManager.Instance.CurrentData;
        if (!saveData.IsNewItem(equipmentId))
        {
            return;
        }

        saveData.SetNewStatus(equipmentId, false);
        SaveManager.Instance.SaveGameSync();
        NotifyEquipmentDataChanged();
    }

    private void LoadEquipmentData()
    {
        CSVManager.Instance.OnLoadingComplete -= LoadEquipmentData;
        equipmentDataById.Clear();

        List<Dictionary<string, object>> table = CSVManager.Instance.GetTable("EquipmentTable");
        if (table == null)
        {
            Debug.LogError("[EquipmentManager] EquipmentTable is missing. Check CSVManager file names.");
            return;
        }

        foreach (Dictionary<string, object> row in table)
        {
            try
            {
                EquipmentData data = ParseEquipmentData(row);
                if (!equipmentDataById.ContainsKey(data.ID))
                {
                    equipmentDataById.Add(data.ID, data);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[EquipmentManager] Failed to parse equipment data. ID: {row["ID"]}, Error: {e.Message}");
            }
        }

        IsInitialized = true;
        OnDataInitialized?.Invoke();

        Debug.Log($"[EquipmentManager] Equipment table loaded. Count: {equipmentDataById.Count}");
    }

    private static EquipmentData ParseEquipmentData(Dictionary<string, object> row)
    {
        return new EquipmentData
        {
            ID = row["ID"].ToString(),
            Name = row["Name"].ToString(),
            EquipType = (EquipmentType)Enum.Parse(typeof(EquipmentType), row["EquipType"].ToString()),
            Grade = (GradeType)Enum.Parse(typeof(GradeType), row["Grade"].ToString()),
            Tier = int.Parse(row["Tier"].ToString()),
            EquipStatType_1 = (StatType)Enum.Parse(typeof(StatType), row["EquipStatType_1"].ToString()),
            EquipStatValue_1 = double.Parse(row["EquipStatValue_1"].ToString()),
            EquipStatType_2 = (StatType)Enum.Parse(typeof(StatType), row["EquipStatType_2"].ToString()),
            EquipStatValue_2 = double.Parse(row["EquipStatValue_2"].ToString()),
            OwnStatType = (StatType)Enum.Parse(typeof(StatType), row["OwnStatType"].ToString()),
            OwnStatValue = double.Parse(row["OwnStatValue"].ToString()),
            NextID = row["NextID"].ToString(),
            NeedCount = int.Parse(row["NeedCount"].ToString()),
            IconName = row["IconName"].ToString()
        };
    }

    private static bool CanSynthesize(EquipmentData data)
    {
        return data != null &&
               data.NeedCount > 0 &&
               !string.IsNullOrEmpty(data.NextID) &&
               data.NextID != "Max";
    }
}
