using System;

public class UpgradeData
{
    public string ID;
    public string Name;
    public float BaseValue;
    public float IncreasePerLevel;
    public double BaseCost;
    public float CostMultiplier;
    public int MaxLevel;
    public bool IsPercentageStat;

    public float GetValue(int level)
    {
        return BaseValue + level * IncreasePerLevel;
    }

    public double GetCost(int level)
    {
        return BaseCost * Math.Pow(CostMultiplier, level);
    }
}
