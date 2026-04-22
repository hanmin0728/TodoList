using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeCell : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI statText;
    public TextMeshProUGUI costText;
    public Button upgradeButton;

    private UpgradeData data;
    private int currentLevel = 0;

    public void Setup(UpgradeData upgradeData)
    {
        data = upgradeData;
        nameText.text = data.Name;
        UpdateUI();
    }

    public void OnUpgradeClick()
    {
        if (currentLevel < data.MaxLevel)
        {
            currentLevel++;
            UpdateUI();
            // 재화 차감 및 실제 능력치 반영 로직 추가
        }
    }

    public void UpdateUI()
    {
        float currentValue = data.BaseValue + (currentLevel * data.IncreasePerLevel);
        float nextValue = data.BaseValue + ((currentLevel + 1) * data.IncreasePerLevel);
        double currentCost = data.BaseCost * Mathf.Pow(data.CostMultiplier, currentLevel);

        statText.text = $"{currentValue} > <color=#FF0000>{nextValue}</color>";

        if (currentLevel >= data.MaxLevel)
            costText.text = "MAX";
        else
            costText.text = $"Cost: {currentCost:N0}"; // N0: 천단위 콤마
    }

}
