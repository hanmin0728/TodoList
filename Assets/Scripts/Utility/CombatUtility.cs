using UnityEngine;

public static class CombatUtility
{
    /// <summary>
    /// 치명타 확률을 계산하여 최종 데미지와 치명타 여부를 반환합니다.
    /// </summary>
    /// <returns> (최종 데미지, 치명타 여부) </returns>
    public static (float damage, bool isCritical) CalculateDamage(float baseDamage, float critChance, float critMultiplier)
    {
        // Random.value는 0.0 ~ 1.0 사이의 값을 반환합니다.
        bool isCritical = Random.value <= critChance;

        // 치명타라면 배율을 곱하고, 아니면 기본 데미지 반환
        float finalDamage = isCritical ? baseDamage * critMultiplier : baseDamage;

        return (finalDamage, isCritical);
    }
}