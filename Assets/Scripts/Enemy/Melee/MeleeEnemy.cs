using UnityEngine;

public class MeleeEnemy : EnemyBase
{
    public override void PerformAttack()
    {
        Debug.Log("공격애니메이션 실행");
    }
}
