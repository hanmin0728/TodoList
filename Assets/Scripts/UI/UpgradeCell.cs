using Mono.Cecil;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeCell : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI statBeforeText;
    public TextMeshProUGUI statAfterText;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI upgradeText;

    public Sprite normalSprite; 
    public Sprite lockedSprite;

    private Color normalTextColor = Color.white;
    private Color lockedTextColor = Color.red;

    public Image buttonImage;
    public LongClickButton longClickButton;

    private UpgradeData data;


    private int CurrentLevel
    {
        get => SaveManager.Instance.CurrentData.GetUpgradeLevel(data.ID);
        set => SaveManager.Instance.CurrentData.SetUpgradeLevel(data.ID, value);
    }

    /// <summary>
    /// 매니저가 이 셀을 생성할 때 한 번만 호출하는 세팅 함수
    /// </summary>
    public void Setup(UpgradeData upgradeData, Sprite icon)
    {
        data = upgradeData;

        if (iconImage != null)
            iconImage.sprite = icon;

        longClickButton.onLongClick = null;
        longClickButton.onLongClick += OnUpgradeClick;

        CurrencyManager.Instance.OnGoldChanged += RefreshVisual;

        UpdateUI();
    }

    public void OnUpgradeClick()
    {
        if (CurrentLevel >= data.MaxLevel) return;

        double cost = data.GetCost(CurrentLevel);
      
        if (CurrencyManager.Instance.TryUpgrade(cost))
        {
            CurrentLevel++;
            UpdateUI();
            longClickButton.SetChanged();
        }
        
    }

    public void UpdateUI()
    {
        nameText.text = data.Name;

        int level = CurrentLevel;
        float currentValue = data.GetValue(level);
        float nextValue = data.GetValue(level + 1);
        double cost = data.GetCost(level);

        levelText.text = (level >= data.MaxLevel) ? "MAX" : level.ToString();

        if (data.IsPercentageStat)
        {
            statBeforeText.text = currentValue.ToString("F1") + "%";
            statAfterText.text = (level >= data.MaxLevel) ? "" : nextValue.ToString("F1") + "%";
        }
        else
        {
            // 기존의 거대 숫자 포맷터 사용
            statBeforeText.text = CurrencyFormatter.Format(currentValue);
            statAfterText.text = (level >= data.MaxLevel) ? "" : CurrencyFormatter.Format(nextValue);
        }

        costText.text = (level >= data.MaxLevel) ? "MAX" : CurrencyFormatter.Format(cost);

        RefreshVisual(SaveManager.Instance.CurrentData.Gold);
    }

    private void RefreshVisual(double currentGold)
    {
        bool isMax = CurrentLevel >= data.MaxLevel;
        bool canAfford = currentGold >= data.GetCost(CurrentLevel) && !isMax;

        buttonImage.sprite = canAfford ? normalSprite : lockedSprite;
        upgradeText.color = canAfford ? normalTextColor : lockedTextColor;
        costText.color = canAfford ? normalTextColor : lockedTextColor;
    }

    private void OnDestroy()
    {
        // 메모리 누수 방지 (구독 해제)
        if (CurrencyManager.Instance != null)
            CurrencyManager.Instance.OnGoldChanged -= RefreshVisual;
    }

}
