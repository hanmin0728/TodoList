using UnityEngine;

public sealed class MeleeEnemy : EnemyBase
{
    public override void PerformAttack()
    {
        IsAttackAnimationFinished = false;
        Anim.Play(EnemyAnimHash.isAttack, 0, 0f);
    }

    public override void OnEnemyAttackHit()
    {
        if (TargetPlayer == null) 
            return;

        float distanceX = Mathf.Abs(transform.position.x - TargetPlayer.transform.position.x);

        if (distanceX <= Data.AttackRange)
        {
            TargetPlayer.TakeDamage(Data.Atk);
        }
    }
}


