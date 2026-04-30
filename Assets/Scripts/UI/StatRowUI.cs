using TMPro;
using UnityEngine;

public class StatRowUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI statNameText;
    [SerializeField] private TextMeshProUGUI valueText;

    public void Setup(string statName, double value)
    {
        statNameText.text = GetStatKorName(statName);
        valueText.text = $"{value}%";
    }

    private string GetStatKorName(string engName)
    {
        return engName switch
        {
            "Atk" => "공격력 증가",
            "Hp" => "체력 증가",
            "AtkSpeed" => "공격 속도 증가",
            "CriticalChance" => "크리티컬 확률",
            "CriticalDamage" => "크리티컬 데미지",
            _ => engName
        };
    }
}
