using UnityEngine;
public static class EnemyAnimHash
{
    public static readonly int isWalk = Animator.StringToHash("walk");
    public static readonly int isIdle = Animator.StringToHash("idle");
    public static readonly int isAttack = Animator.StringToHash("attack");
    public static readonly int isHit = Animator.StringToHash("hit");
    public static readonly int isDie = Animator.StringToHash("die");
}