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

    // 현재 레벨에 따른 능력치 계산
    public float GetValue(int level) => BaseValue + (level * IncreasePerLevel);
    
    // 현재 레벨에 따른 비용 계산
    public double GetCost(int level) => BaseCost * Mathf.Pow(CostMultiplier, level);
}
