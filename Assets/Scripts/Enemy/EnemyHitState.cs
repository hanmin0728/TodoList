using System.Collections;
using UnityEngine;

public class EnemyHitState : BaseState<EnemyBase>
{
    private float _hitStunDuration = 0.2f; 
    private float _timer;

    public EnemyHitState(EnemyBase owner, StateMachine<EnemyBase> stateMachine) : base(owner, stateMachine)
    {
    }
    public override void Enter()
    {
        _timer = _hitStunDuration;
        owner.Anim.SetTrigger(EnemyAnimHash.isHit);

        owner.Rigid2D.linearVelocity = new Vector2(owner.LastHitForce, 0);

        owner.StartCoroutine(FlashCo());
    }

    private IEnumerator FlashCo()
    {
        // 알파값(A)을 0.5 정도로 낮춰서 투명하게 만듭니다.
        Color originalColor = owner.Sprite.color;
        owner.Sprite.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.5f);

        // 0.1초 대기
        yield return new WaitForSeconds(0.1f);

        // 다시 원래 색상으로 복구
        owner.Sprite.color = originalColor;
    }
    public override void Exit()
    {
        owner.Rigid2D.linearVelocity = Vector2.zero;
    }

    public override void Update()
    {
        _timer -= Time.deltaTime;
        if (_timer <= 0)
        {
            stateMachine.ChangeState(owner.ChaseState);
        }
    }

}
