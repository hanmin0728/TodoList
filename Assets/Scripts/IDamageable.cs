using UnityEngine;

public interface IDamageable
{
    // 데미지를 입을 때 호출 (데미지 양, 피격 위치)
    void OnDamage(float damage);
}