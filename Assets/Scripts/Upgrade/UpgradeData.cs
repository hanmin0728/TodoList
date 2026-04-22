using UnityEngine;

public class UpgradeData 
{
    public string ID;
    public string Name;
    public float BaseValue;         // 기본 수치
    public float IncreasePerLevel;  // 레벨당 증가 수치
    public double BaseCost;           // 시작 비용
    public float CostMultiplier;    // 비용 증가 계수
    public int MaxLevel;

    public float GetValueAtLevel(int level)
    {
        return BaseValue + (level * IncreasePerLevel);
    }

    // 💡 현재 레벨을 넣으면 다음 강화 비용을 반환하는 함수 (복리 공식)
    public long GetCostAtLevel(int level)
    {
        // 공식: 시작비용 * (증가계수 ^ 현재레벨)
        return (long)(BaseCost * Mathf.Pow(CostMultiplier, level));
    }
}
