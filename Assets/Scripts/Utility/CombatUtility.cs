using UnityEngine;

public static class CombatUtility
{
    /// <summary>
    /// 치명타 확률을 계산하여 최종 데미지와 치명타 여부를 반환합니다.
    /// </summary>
    /// <param name="baseDamage">기본 데미지</param>
    /// <param name="criticalChance">치명타 확률 (1 ~ 100)</param>
    /// <returns> (최종 데미지, 치명타 여부) </returns>
    public static (float damage, bool isCritical) CalculateDamage(float baseDamage, float criticalChance)
    {
        bool isCritical = Random.Range(0f, 100f) <= criticalChance;

        if (isCritical)
        {
            float finalMultiplier = 2f;
            float finalDamage = baseDamage * finalMultiplier;

            return (finalDamage, true);
        }
        else
        {
            return (baseDamage, false);
        }
    }
}