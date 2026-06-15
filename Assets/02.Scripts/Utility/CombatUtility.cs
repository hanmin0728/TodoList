using UnityEngine;

public static class CombatUtility
{
    public static (float damage, bool isCritical) CalculateDamage(float baseDamage, float criticalChance, float criticalDamageMultiplier = 2f)
    {
        bool isCritical = Random.Range(0f, 100f) <= criticalChance;
        if (!isCritical)
        {
            return (baseDamage, false);
        }

        float multiplier = Mathf.Max(1f, criticalDamageMultiplier);
        return (baseDamage * multiplier, true);
    }
}
