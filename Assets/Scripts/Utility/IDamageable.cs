using UnityEngine;

public interface IDamageable
{
    // 데미지를 입을 때 호출 (데미지 양, 넉백 수치)
    public void OnDamage(float damage, float knockbackForce);
}