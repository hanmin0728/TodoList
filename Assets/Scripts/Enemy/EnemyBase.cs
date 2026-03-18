using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public abstract class EnemyBase : MonoBehaviour, IDamageable
{
    public StateMachine<EnemyBase> StateMachine { get; protected set; }

    #region 상태 패턴
    public EnemyChaseState ChaseState { get; protected set; }
    public EnemyAttackState AttackState { get; protected set; }
    public EnemyHitState HitState { get; protected set; }
    public EnemyDieState DieState { get; protected set; }
    #endregion

    public EnemyData data;

    protected float currentHp;
    protected bool IsDead = false;

    public float LastHitForce { get; private set; } 

    public Animator Anim { get; protected set; }
    public SpriteRenderer Sprite { get; protected set; }
    public Rigidbody2D Rigid2D { get; protected set; }

    public Poolable Poolable { get; private set; }

    // 공격 애니메이션 실행 완료 여부
    public bool IsAttackAnimationFinished { get; set; } = true;

    public void Awake()
    {
        Anim = GetComponent<Animator>();
        Rigid2D = GetComponent<Rigidbody2D>();
        Sprite = GetComponent<SpriteRenderer>();
        Poolable = GetComponent<Poolable>();

        StateMachine = new StateMachine<EnemyBase>();

        ChaseState = new EnemyChaseState(this, StateMachine);
        AttackState = new EnemyAttackState(this, StateMachine);
        HitState = new EnemyHitState(this, StateMachine);
        DieState = new EnemyDieState(this, StateMachine);
    }
    protected virtual void OnEnable()
    {
        IsDead = false;
        if (Sprite != null) Sprite.color = Color.white;
    }

    public void Init(EnemyData newData)
    {
        data = newData;
        currentHp = data.hp; // 최대 체력 정보를 가져와 현재 체력 초기화
        IsAttackAnimationFinished = false;
        StateMachine.Initialize(ChaseState);
    }

    protected virtual void Update()
    {
        if (StateMachine.CurrentState != null)
            StateMachine.CurrentState.Update();
    }

 


    public virtual void OnDamage(float damage, float knockBackForce)
    {
        if (IsDead) return;
        currentHp -= damage;
        LastHitForce = knockBackForce;
        
        Rigid2D.linearVelocity = new Vector2(LastHitForce, 0);
        PlayHitEffect();

        if (currentHp <= 0)
        {
            IsDead = true;
            StateMachine.ChangeState(DieState); // 죽음 상태로 전환 
            //리워드 지급 및 애니메이션 실행은 DieState에서 처리
        }
        else
        {
            StateMachine.ChangeState(HitState);
        }
    }


    public virtual void PlayHitEffect()
    {
        StartCoroutine(FlashCo());
    }

    private IEnumerator FlashCo()
    {
        if (Sprite == null) yield break;

        Color originalColor = Sprite.color;

        // 알파값 조절
        Sprite.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.5f);

        yield return new WaitForSeconds(0.1f);

        Sprite.color = originalColor;
    }

    /// <summary>
    /// 자식 클래스에서 본인만의 공격 방식 구현
    /// </summary>
    public abstract void PerformAttack();

    /// <summary>
    /// 애니메이션 이벤트에서 호출
    /// </summary>
    public virtual void OnEnemyAttackHit()
    {
    }

    /// <summary>
    /// 애니메이션 이벤트에서 호출
    /// </summary>
    public void OnAttackSequenceFinished()
    {
        IsAttackAnimationFinished = true;
    }

    /// <summary>
    /// 애니메이션 이벤트에서 호출
    /// </summary>
    public void OnDieAnimationEnd()
    {
        if (Poolable == null)
        {
            Poolable = GetComponent<Poolable>();
        }

        Poolable.Release();
    }
}
