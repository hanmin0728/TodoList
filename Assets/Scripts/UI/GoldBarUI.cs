using TMPro;
using UnityEngine;

public class GoldBarUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _goldText;

    private void Start()
    {
        // 이미 로드가 끝났다면 바로 표시
        if (SaveManager.Instance.CurrentData != null)
        {
            UpdateUI();
        }

        CurrencyManager.Instance.OnGoldChanged += UpdateGoldUI;

        SaveManager.Instance.OnDataLoaded += UpdateUI;
    }

    private void OnDestroy()
    {
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnGoldChanged -= UpdateGoldUI;
        }
    }

    /// <summary>
    /// CurrencyManager OnGoldChanged 액션 실행시 실행
    /// </summary>
    private void UpdateGoldUI(double currentGold)
    {
        _goldText.text = CurrencyFormatter.Format(currentGold);
    }

    private void UpdateUI()
    {
        UpdateGoldUI(SaveManager.Instance.CurrentData.Gold);
    }
}
