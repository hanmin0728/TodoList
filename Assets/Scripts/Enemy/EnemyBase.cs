using System.Collections;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour, IDamageable
{
    private static readonly WaitForSeconds HitFlashDelay = new WaitForSeconds(0.1f);

    private Coroutine flashCoroutine;

    protected float currentHp;
    protected bool isDead;

    public StateMachine<EnemyBase> StateMachine { get; protected set; }
    public EnemyChaseState ChaseState { get; protected set; }
    public EnemyAttackState AttackState { get; protected set; }
    public EnemyHitState HitState { get; protected set; }
    public EnemyDieState DieState { get; protected set; }

    public Animator Anim { get; protected set; }
    public SpriteRenderer Sprite { get; protected set; }
    public Rigidbody2D Rigid2D { get; protected set; }
    public Collider2D Collider2D { get; private set; }
    public Poolable Poolable { get; private set; }
    public PlayerController TargetPlayer { get; private set; }

    public EnemyData Data { get; private set; }
    public bool IsAttackAnimationFinished { get; set; } = true;

    protected virtual void Awake()
    {
        Anim = GetComponent<Animator>();
        Rigid2D = GetComponent<Rigidbody2D>();
        Sprite = GetComponent<SpriteRenderer>();
        Collider2D = GetComponent<Collider2D>();
        Poolable = GetComponent<Poolable>();

        StateMachine = new StateMachine<EnemyBase>();
        ChaseState = new EnemyChaseState(this, StateMachine);
        AttackState = new EnemyAttackState(this, StateMachine);
        HitState = new EnemyHitState(this, StateMachine);
        DieState = new EnemyDieState(this, StateMachine);
    }

    protected virtual void OnEnable()
    {
        isDead = false;
        IsAttackAnimationFinished = true;

        if (Sprite != null)
        {
            Sprite.color = Color.white;
        }

        if (Collider2D != null)
        {
            Collider2D.enabled = true;
        }
    }

    protected virtual void Update()
    {
        StateMachine.Update();
    }

    public void SpawnInit(EnemyData newData)
    {
        Data = newData;
        currentHp = Data.Hp;
        isDead = false;
        IsAttackAnimationFinished = true;

        TargetPlayer = GameManager.Instance.Player;

        if (Collider2D != null)
        {
            Collider2D.enabled = true;
        }

        StateMachine.Initialize(ChaseState);
    }

    public virtual void OnDamage(float damage, float knockBackForce)
    {
        if (isDead || damage <= 0f)
        {
            return;
        }

        currentHp -= damage;

        PlayHitEffect();

        if (currentHp <= 0f)
        {
            Die();
            return;
        }

        Rigid2D.linearVelocity = new Vector2(knockBackForce, 0f);
        StateMachine.ChangeState(HitState);
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;
        CurrencyManager.Instance.AddGold(Data.GoldReward);
        EnemySpawner.Instance.OnEnemyDeath();

        StateMachine.ChangeState(DieState);
    }

    public virtual void PlayHitEffect()
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }

        flashCoroutine = StartCoroutine(FlashCo());
    }
    private IEnumerator FlashCo()
    {
        if (Sprite == null) yield break;

        Color originalColor = Sprite.color;
        Sprite.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.5f);

        yield return HitFlashDelay;

        Sprite.color = originalColor;
        flashCoroutine = null;
    }


    public bool IsTargetInAttackRange()
    {
        if (TargetPlayer == null) return false;

        float distanceX = Mathf.Abs(transform.position.x - TargetPlayer.transform.position.x);
        return distanceX <= Data.AttackRange;
    }

    /// <summary>
    /// 각 적의 공격 방식(근거리/원거리)에 따라 구현
    /// </summary>
    public abstract void PerformAttack();

    public virtual void OnEnemyAttackHit()
    {
    }

    public void OnAttackSequenceFinished()
    {
        IsAttackAnimationFinished = true;
    }

    public void OnDieAnimation()
    {
        Poolable?.Release();
    }
}


