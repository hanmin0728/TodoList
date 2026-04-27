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
    private bool _isInitialized = false;

    void Start()
    {
        if (CSVManager.Instance.IsInitialized)
        {
            InitializeUI();
        }
        else
        {
            // 아직 로딩 중이라면 완료 이벤트에 등록
            CSVManager.Instance.OnLoadingComplete += InitializeUI;
        }
    }

    /// <summary>
    /// 처음 UI 켰을때만 생성 
    /// </summary>
    private void InitializeUI()
    {
        CSVManager.Instance.OnLoadingComplete -= InitializeUI;

        if (_isInitialized) return;

        var table = CSVManager.Instance.GetTable("UpgradeTable");
        foreach (var row in table)
        {
            GameObject obj = Instantiate(upgradeCell, contentParent);
            var itemScript = obj.GetComponent<UpgradeCell>();

            // 데이터 파싱 및 초기 셋업
            UpgradeData data = ParseRowToData(row);

            Sprite matchedIcon = GetIconByID(data.ID);
            itemScript.Setup(data, matchedIcon);

            _activeCells.Add(itemScript);
        }

        _isInitialized = true;
    }

    private void OnEnable()
    {
        if (!_isInitialized) return;
        RefreshAllCells();
    }

    public void RefreshAllCells()
    {
        // 새로 생성하는 게 아니라, 이미 있는 셀들의 텍스트만 갱신
        foreach (var cell in _activeCells)
        {
            // UpdateUI()는 내부 데이터(레벨 등)를 기반으로 텍스트만 다시 그리는 함수
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

    //Dictionary를 Data 객체로 변환
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
}
