using UnityEngine;
public static class EnemyAnimHash
{
    // 게임 시작시 한번 숫자로 변환
    public static readonly int isIdle = Animator.StringToHash("isIdle");
    public static readonly int isAttack = Animator.StringToHash("isAttack");
    public static readonly int isHit = Animator.StringToHash("isHit");
    public static readonly int isDie = Animator.StringToHash("isDie");
}