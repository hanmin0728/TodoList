using System.Collections;
using UnityEngine;
using UnityEngine.U2D.Animation;

public sealed class PlayerController : MonoBehaviour
{
    private const string AttackStatId = "Atk";
    private const string HpStatId = "Hp";
    private const string AttackSpeedStatId = "AtkSpeed";
    private const string CriticalChanceStatId = "CriticalChance";
    private const string CriticalDamageStatId = "CriticalDamage";

    private static readonly WaitForSeconds HitFlashDelay = new WaitForSeconds(0.1f);

    [Header("Player Data")]
    [SerializeField] private PlayerData data;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private int maxAttackHitCount = 16;

    [Header("Combat Offsets")]
    [SerializeField] private Vector2 attackOffset = new Vector2(0.5f, 0f);

    private ContactFilter2D enemyContactFilter;
    private Collider2D[] attackHitBuffer;
    private Transform cachedTransform;
    private Coroutine flashCoroutine;
    private Color[] _defaultColors;

    // 실시간 연산을 피하기 위한 스탯 캐싱 변수들
    private float currentHp;
    private float cachedMaxHp;
    private float cachedDamage;
    private float cachedCriticalChance;
    private float cachedCriticalDamage;
    private float cachedAttackDelay;

    public StateMachine<PlayerController> StateMachine { get; private set; }
    public PlayerMoveState MoveState { get; private set; }
    public PlayerAttackState AttackState { get; private set; }
    
    public Animator Anim;

    private SpriteRenderer[] spriteRenderers;

    private SpriteResolver[] spriteResolvers;

    public PlayerData Data => data;
 
    public int AnimAttackHash { get; private set; }
    public int AnimMoveHash { get; private set; }
    public int AnimIdleHash { get; private set; }
    public bool IsAttackAnimationFinished { get; set; } = true;

    public float CurrentAttackDelay => cachedAttackDelay;


    private void Awake()
    {
        cachedTransform = transform;

        spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
        spriteResolvers = GetComponentsInChildren<SpriteResolver>(true);

        AnimAttackHash = Animator.StringToHash(data.AttackAnimationParam);
        AnimMoveHash = Animator.StringToHash(data.MoveAnimationParam);
        AnimMoveHash = Animator.StringToHash(data.MoveAnimationParam);
        AnimIdleHash = Animator.StringToHash(data.IdleAnimationParam); 

        attackHitBuffer = new Collider2D[Mathf.Max(1, maxAttackHitCount)];

        StateMachine = new StateMachine<PlayerController>();
        MoveState = new PlayerMoveState(this, StateMachine);
        AttackState = new PlayerAttackState(this, StateMachine);

        enemyContactFilter = new ContactFilter2D();
        enemyContactFilter.useLayerMask = true; 
        enemyContactFilter.SetLayerMask(enemyLayer);

        _defaultColors = new Color[spriteRenderers.Length];
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            _defaultColors[i] = spriteRenderers[i].color;
        }

        GameManager.Instance.RegisterPlayer(this);
    }

    private void Start()
    {
        if (UpgradeManager.Instance.IsInitialized)
        {
            Init();
        }
        else
        {
            UpgradeManager.Instance.OnUpgradeDataLoaded += Init;
        }
    }

    private void Update()
    {
        StateMachine.Update();
    }


    public void Init()
    {
        UpgradeManager.Instance.OnUpgradeDataLoaded -= Init;
        UpdateStatsCache(); 
        currentHp = cachedMaxHp;
        StateMachine.Initialize(MoveState);
    }

    public void UpdateStatsCache()
    {
        cachedMaxHp = GetCalculatedStat(HpStatId, data.Hp);
        cachedDamage = GetCalculatedStat(AttackStatId, data.AttackDamage);
        cachedCriticalChance = GetCalculatedStat(CriticalChanceStatId, 0f);
        cachedCriticalDamage = GetCalculatedStat(CriticalDamageStatId, 2f);
        cachedAttackDelay = Mathf.Max(0.1f, data.AttackDelay - GetCalculatedStat(AttackSpeedStatId, 0f));
    }


    public bool CheckEnemyInRange()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(GetAttackCheckPosition(), data.AttackRange, enemyLayer);
        foreach (var hit in hits)
        {
            EnemyBase enemy = hit.GetComponent<EnemyBase>();

            if (enemy != null)
            {
                if (!enemy.IsDead && !enemy.IsSpawning)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void OnAttackSequenceFinished()
    {
        IsAttackAnimationFinished = true;
    }

    public void OnAttackHit()
    {
        int hitCount = Physics2D.OverlapCircle(GetAttackCheckPosition(), data.AttackRange, enemyContactFilter, attackHitBuffer);

        for (int i = 0; i < hitCount; i++)
        {
            Collider2D hit = attackHitBuffer[i];

            if (hit == null || !hit.TryGetComponent(out IDamageable enemy)) continue;

            (float finalDamage, bool isCritical) = CombatUtility.CalculateDamage(cachedDamage, cachedCriticalChance, cachedCriticalDamage);
           
            enemy.OnDamage(finalDamage, data.KnockBackForce);

            TextType popupType = isCritical ? TextType.CriticalDamage : TextType.NormalDamage;
            FloatingTextPopUpManager.Instance.Show(finalDamage, hit.transform, popupType);
        }
    }

  
    public void TakeDamage(float damage)
    {
        if (damage <= 0f)
            return;

        currentHp -= damage;

        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }

        if (currentHp <= 0f)
        {
            Die();
        }

        flashCoroutine = StartCoroutine(PlayerFlashCo());
    }

    public void MoveRoutine()
    {
        GameManager.Instance.MoveBackground(data.ScrollSpeed);
    }

    private float GetCalculatedStat(string id, float fallbackBaseValue)
    {
        if (!UpgradeManager.Instance.IsInitialized)
        {
            return fallbackBaseValue;
        }

        UpgradeData upgradeData = UpgradeManager.Instance.GetUpgradeData(id);
        if (upgradeData == null)
        {
            return fallbackBaseValue;
        }

        int level = SaveManager.Instance.CurrentData.GetUpgradeLevel(id);
        return upgradeData.GetValue(level);
    }

    private Vector2 GetAttackCheckPosition()
    {
        return (Vector2)cachedTransform.position + attackOffset;
    }

    private IEnumerator PlayerFlashCo()
    {
        foreach (var sr in spriteRenderers)
        {
            sr.color = Color.red;
        }

        yield return HitFlashDelay;

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].color = _defaultColors[i];
        }

        flashCoroutine = null;
    }

    private void Die()
    {
        Debug.Log("[PlayerController] Player died.");
    }
    private void OnDrawGizmosSelected()
    {
        if (data == null) return;

        Gizmos.color = Color.red;

        Vector2 checkPosition = Application.isPlaying
            ? GetAttackCheckPosition()
            : (Vector2)transform.position + attackOffset;

        Gizmos.DrawWireSphere(checkPosition, data.AttackRange);
    }
}
