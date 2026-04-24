using TMPro;
using UnityEngine;

public class GoldBarUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _goldText;

    private void Start()
    {
        UpdateGoldUI(SaveManager.Instance.CurrentData.Gold);
     
        CurrencyManager.Instance.OnGoldChanged += UpdateGoldUI;
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
    private void UpdateGoldUI(long currentGold)
    {
        _goldText.text = currentGold.ToString("N0");
    }
}
