using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeCell : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI statBeforeText;
    [SerializeField] private TextMeshProUGUI statAfterText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI upgradeText;
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite lockedSprite;
    [SerializeField] private Image buttonImage;
    [SerializeField] private LongClickButton longClickButton;

    private static readonly Color NormalTextColor = Color.white;
    private static readonly Color LockedTextColor = Color.red;

    private UpgradeData data;

    private int CurrentLevel
    {
        get => SaveManager.Instance.CurrentData.GetUpgradeLevel(data.ID);
        set => SaveManager.Instance.CurrentData.SetUpgradeLevel(data.ID, value);
    }

    public void Setup(UpgradeData upgradeData, Sprite icon)
    {
        data = upgradeData;

        if (iconImage != null)
        {
            iconImage.sprite = icon;
        }

        longClickButton.SetClickAction(OnUpgradeClick);

        UpgradeManager.Instance.OnUpgradeChanged += HandleUpgradeChanged;
        CurrencyManager.Instance.OnGoldChanged += RefreshVisual;

        UpdateUI();
    }

    public void OnUpgradeClick()
    {
        UpgradeManager.Instance.TryUpgrade(data.ID);
    }
    private void HandleUpgradeChanged(string id)
    {
        // 내 ID와 일치할 때만 갱신
        if (id == data.ID) UpdateUI();
    }
    public void UpdateUI()
    {
        if (data == null)
        {
            return;
        }

        int level = CurrentLevel;
        bool isMax = level >= data.MaxLevel;
        int nextLevel = isMax ? level : level + 1;

        nameText.text = data.Name;
        if (isMax)
        {
            levelText.text = "MAX";
        }
        else
        {
            levelText.SetText("{0}", level);
        }

        float currentValue = data.GetValue(level);
        float nextValue = data.GetValue(nextLevel);

        if (data.IsPercentageStat)
        {
            statBeforeText.SetText("{0:0.0}%", currentValue);
            if (isMax)
            {
                statAfterText.text = string.Empty;
            }
            else
            {
                statAfterText.SetText("{0:0.0}%", nextValue);
            }
        }
        else
        {
            statBeforeText.text = CurrencyFormatter.Format(currentValue);
            statAfterText.text = isMax ? string.Empty : CurrencyFormatter.Format(nextValue);
        }

        costText.text = isMax ? "MAX" : CurrencyFormatter.Format(data.GetCost(level));
        RefreshVisual(SaveManager.Instance.CurrentData.GetGold());
    }

    private void RefreshVisual(double currentGold)
    {
        if (data == null)
        {
            return;
        }

        int level = CurrentLevel;
        bool isMax = level >= data.MaxLevel;
        bool canAfford = !isMax && currentGold >= data.GetCost(level);

        buttonImage.sprite = canAfford ? normalSprite : lockedSprite;
        upgradeText.color = canAfford ? NormalTextColor : LockedTextColor;
        costText.color = canAfford ? NormalTextColor : LockedTextColor;
    }

    private void OnDestroy()
    {
        if (UpgradeManager.Instance != null)
            UpgradeManager.Instance.OnUpgradeChanged -= HandleUpgradeChanged;

        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnGoldChanged -= RefreshVisual;
        }
    }
}
