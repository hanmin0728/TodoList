using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct UpgradeIconMapping
{
    public string statID;      // CSV에 적힌 ID
    public Sprite iconSprite; 
}

public class UpgradeUIManager : MonoBehaviour
{
    [SerializeField] private GameObject upgradeCell;
    [SerializeField] private Transform contentParent;

    [SerializeField] private List<UpgradeIconMapping> iconMappings = new List<UpgradeIconMapping>();

    private List<UpgradeCell> _activeCells = new List<UpgradeCell>();

    void Start()
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

    /// <summary>
    /// 처음 UI 켰을때만 생성 
    /// </summary>
    private void InitializeUI()
    {
        UpgradeManager.Instance.OnUpgradeDataLoaded -= InitializeUI;

        var allUpgrades = UpgradeManager.Instance.UpgradeDictionary.Values;

        foreach (var data in allUpgrades)
        {
            GameObject obj = Instantiate(upgradeCell, contentParent);
            var itemScript = obj.GetComponent<UpgradeCell>();

            Sprite matchedIcon = GetIconByID(data.ID);
            itemScript.Setup(data, matchedIcon);

            _activeCells.Add(itemScript);
        }
    }

    private void OnEnable()
    {
        RefreshAllCells();
    }

    public void RefreshAllCells()
    {
        // 새로 생성하는 게 아니라, 이미 있는 셀들의 텍스트만 갱신
        foreach (var cell in _activeCells)
        {
            cell.UpdateUI();
        }
    }

    /// <summary>
    /// ID를 넣으면 매핑된 아이콘을 반환
    /// </summary>
    private Sprite GetIconByID(string id)
    {
        foreach (var mapping in iconMappings)
        {
            if (mapping.statID == id)
                return mapping.iconSprite;
        }

        Debug.LogWarning($"[{id}]에 해당하는 아이콘을 찾지 못함");

        return null; 
    }


}
