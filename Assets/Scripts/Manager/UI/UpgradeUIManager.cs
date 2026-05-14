using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct UpgradeIconMapping
{
    public string statID;
    public Sprite iconSprite;
}

public sealed class UpgradeUIManager : MonoBehaviour
{
    [SerializeField] private GameObject upgradeCell;
    [SerializeField] private Transform contentParent;
    [SerializeField] private List<UpgradeIconMapping> iconMappings = new List<UpgradeIconMapping>();

    private readonly List<UpgradeCell> activeCells = new List<UpgradeCell>();
    private readonly Dictionary<string, Sprite> iconByStatId = new Dictionary<string, Sprite>();

    private void Awake()
    {
        BuildIconCache();
    }

    private void Start()
    {
        if (UpgradeManager.Instance.IsInitialized)
        {
            InitializeUI();
        }
        else
        {
            UpgradeManager.Instance.OnUpgradeDataLoaded += InitializeUI;
        }
    }

    private void OnEnable()
    {
        RefreshAllCells();
    }

    public void RefreshAllCells()
    {
        for (int i = 0; i < activeCells.Count; i++)
        {
            activeCells[i].UpdateUI();
        }
    }

    private void InitializeUI()
    {
        UpgradeManager.Instance.OnUpgradeDataLoaded -= InitializeUI;

        foreach (UpgradeData data in UpgradeManager.Instance.GetAllUpgradeData())
        {
            GameObject obj = Instantiate(upgradeCell, contentParent);
            UpgradeCell cell = obj.GetComponent<UpgradeCell>();
            cell.Setup(data, GetIconByID(data.ID));
            activeCells.Add(cell);
        }
    }

    private Sprite GetIconByID(string id)
    {
        if (iconByStatId.TryGetValue(id, out Sprite icon))
        {
            return icon;
        }

        Debug.LogWarning($"[UpgradeUIManager] Missing upgrade icon. ID: {id}");
        return null;
    }

    private void BuildIconCache()
    {
        iconByStatId.Clear();

        for (int i = 0; i < iconMappings.Count; i++)
        {
            UpgradeIconMapping mapping = iconMappings[i];
            if (string.IsNullOrEmpty(mapping.statID) || iconByStatId.ContainsKey(mapping.statID))
            {
                continue;
            }

            iconByStatId.Add(mapping.statID, mapping.iconSprite);
        }
    }
}
