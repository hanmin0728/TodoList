using UnityEngine;

public class MeleeEnemy : EnemyBase
{
    public override void PerformAttack()
    {
        Anim.SetTrigger(EnemyAnimHash.isAttack);
    }

}
