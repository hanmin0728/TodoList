using UnityEngine;

public interface IDamageable
{
    public void OnDamage(float damage, float knockbackForce);
}