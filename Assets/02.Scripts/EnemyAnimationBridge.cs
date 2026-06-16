using UnityEngine;

public class EnemyAnimationBridge : MonoBehaviour
{
    private EnemyBase enemy;

    private void Awake()
    {
        enemy = GetComponentInParent<EnemyBase>();
    }

    public void OnAttackHit()
    {
      if (enemy != null) enemy.OnEnemyAttackHit();
    }

    public void OnAttackSequenceFinished()
    {
        if (enemy != null) enemy.OnAttackSequenceFinished();
    }
    public void OnDieAnimation()
    {
        if (enemy != null) enemy.OnDieAnimation();
    }
}
