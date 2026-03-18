using System.Buffers;
using UnityEngine;

public class MeleeEnemy : EnemyBase
{
    public override void PerformAttack()
    {
        IsAttackAnimationFinished = false;
        Anim.Play(EnemyAnimHash.isAttack, 0, 0f);
    }

    public override void OnEnemyAttackHit()
    {
        float distance = Vector2.Distance(transform.position, GameManager.Instance.Player.transform.position);

        if (distance <= data.attackRange)
        {
            GameManager.Instance.Player.TakeDamage(data.atk);
        }
    }

    public override void PlayHitEffect()
    {
        base.PlayHitEffect(); 
        //근접 공격 소리 추가 
    }

}
