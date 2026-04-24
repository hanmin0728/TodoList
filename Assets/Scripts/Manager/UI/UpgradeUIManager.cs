using System.Collections.Generic;
using UnityEngine;

public class UpgradeUIManager : MonoBehaviour
{
    public GameObject upgradeCell;
    public Transform contentParent;
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
        if (_isInitialized) return;

        var table = CSVManager.Instance.GetTable("UpgradeTable");
        foreach (var row in table)
        {
            GameObject obj = Instantiate(upgradeCell, contentParent);
            var itemScript = obj.GetComponent<UpgradeCell>();

            // 데이터 파싱 및 초기 셋업
            UpgradeData data = ParseRowToData(row);
            itemScript.Setup(data);

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

    //Dictionary를 Data 객체로 변환
    private UpgradeData ParseRowToData(Dictionary<string, object> row)
    {
        return new UpgradeData
        {
            ID = row["ID"].ToString(),
            Name = row["Name"].ToString(),
            BaseValue = float.Parse(row["BaseValue"].ToString()),
            IncreasePerLevel = float.Parse(row["IncreasePerLevel"].ToString()),
            BaseCost = double.Parse(row["BaseCost"].ToString()),
            CostMultiplier = float.Parse(row["CostMultiplier"].ToString()),
            MaxLevel = int.Parse(row["MaxLevel"].ToString())
        };
    }
}
