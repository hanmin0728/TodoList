using TMPro;
using UnityEngine;

public sealed class StatRowUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI statNameText;
    [SerializeField] private TextMeshProUGUI valueText;

    public void Setup(StatType statType, double value)
    {
        statNameText.text = GetDisplayName(statType);
        valueText.text = $"{value}%";
    }

    private static string GetDisplayName(StatType statType)
    {
        switch (statType)
        {
            case StatType.Atk:
                return "공격력 증가";
            case StatType.Hp:
                return "체력 증가";
            case StatType.AtkSpeed:
                return "공격 속도 증가";
            case StatType.CriticalChance:
                return "치명타 확률";
            case StatType.CriticalDamage:
                return "치명타 피해";
            default:
                return statType.ToString();
        }
    }
}
