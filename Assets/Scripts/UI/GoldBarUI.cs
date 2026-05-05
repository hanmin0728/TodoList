using TMPro;
using UnityEngine;

public sealed class GoldBarUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _goldText;

    private void Start()
    {
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

        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.OnDataLoaded -= UpdateUI;
        }
    }

    private void UpdateGoldUI(double currentGold)
    {
        _goldText.text = CurrencyFormatter.Format(currentGold);
    }

    private void UpdateUI()
    {
        UpdateGoldUI(SaveManager.Instance.CurrentData.GetGold());
    }
}
