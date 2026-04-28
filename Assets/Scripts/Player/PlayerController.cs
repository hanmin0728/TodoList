using UnityEngine;
using System.Collections;


public class PlayerController : MonoBehaviour
{
    public StateMachine<PlayerController> StateMachine { get; private set; }

    public PlayerMoveState MoveState { get; private set; }
    public PlayerAttackState AttackState { get; private set; }

    [Header("플레이어 기본 데이터 설정")]
    public PlayerData data;

    public Animator Anim { get; private set; }
    public SpriteRenderer SpriteRenderer { get; private set; }
    
    [SerializeField] private LayerMask enemyLayer;

    public int AnimAttackHash { get; private set; }

    private float currentHp;

    public bool IsAttackAnimationFinished { get; set; } = true;

    public float CurrentDamage => GetCalculatedStat("Atk", data.attackDamage);
    public float MaxHp => GetCalculatedStat("Hp", data.hp);
    public float CurrentCriticalChance => GetCalculatedStat("CriticalChance", 0f);
    public float CurrentCriticalDamage => GetCalculatedStat("CriticalDamage", 2f);
    public float CurrentAttackDelay => Mathf.Max(0.1f, data.attackDelay - GetCalculatedStat("AtkSpeed", 0f));

    private void Awake()
    {
        StateMachine = new StateMachine<PlayerController>();

        MoveState = new PlayerMoveState(this, StateMachine);
        AttackState = new PlayerAttackState(this, StateMachine);

        Anim = GetComponent<Animator>();
        SpriteRenderer = GetComponent<SpriteRenderer>();

        GameManager.Instance.RegisterPlayer(this);

        AnimAttackHash = Animator.StringToHash(data.attacAnimationParam);
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

    public void Init()
    {
        UpgradeManager.Instance.OnUpgradeDataLoaded -= Init;

        currentHp = MaxHp;
        
        StateMachine.Initialize(MoveState);
    }

    private void Update()
    {
        if (StateMachine.CurrentState != null)
        {
            StateMachine.CurrentState.Update();
        }
    }
 

    private float GetCalculatedStat(string id, float fallbackBaseValue)
    {
        // 1. 매니저가 아직 준비 안 됐으면 기본값 반환 (안전장치)
        if (!UpgradeManager.Instance.IsInitialized) return fallbackBaseValue;

        // 2. CSV 데이터를 들고 있는 매니저에서 UpgradeData 가져오기 
        UpgradeData upgradeData = UpgradeManager.Instance.GetUpgradeData(id);
        if (upgradeData == null) return fallbackBaseValue;

        // 3. SaveData에서 현재 레벨 가져오기
        int level = SaveManager.Instance.CurrentData.GetUpgradeLevel(id);

        // 4. 공식으로 최종 능력치 계산
        return upgradeData.GetValue(level);
    }

    /// <summary>
    /// 플레이어 앞에 적이 있는지 체크 후 bool 값 반환
    /// </summary>
    public bool CheckEnemyInRange()
    {
        Vector2 checkPos = (Vector2)transform.position + new Vector2(0.5f, 0);

        Collider2D hit = Physics2D.OverlapCircle(checkPos, data.attackRange, enemyLayer);
        if (hit != null)
        {
            //Debug.Log("감지된 적 이름: " + hit.gameObject.name);
        }
        return hit != null; 
    }

    /// <summary>
    /// 공격 애니메이션 재생 완료시 애니메이션 이벤트에서 호출 
    /// /// </summary>
    public void OnAttackSequenceFinished()
    {
        IsAttackAnimationFinished = true;
    } 

    /// <summary>
    /// 애니메이션 이벤트에서 호출 실제 공격 함수
    /// </summary>
    public void OnAttackHit()
    {
        Vector2 checkPos = (Vector2)transform.position + new Vector2(0.5f, 0);

        // 범위 내의 모든 적 감지 
        Collider2D[] hits = Physics2D.OverlapCircleAll(checkPos, data.attackRange, enemyLayer);

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out IDamageable enemy))
            {
                var result = CombatUtility.CalculateDamage(CurrentDamage, CurrentCriticalChance);
                enemy.OnDamage(result.damage, data.knockBackForce);

                TextType popupType = result.isCritical ? TextType.CriticalDamage : TextType.NormalDamage;
                FloatingTextPopUpManager.Instance.Show(result.damage.ToString("F0"), hit.transform, popupType);
            }
        }
    }


    public void TakeDamage(float damage)
    {
        currentHp -= damage;

        StartCoroutine(PlayerFlashCo());

        if (currentHp <= 0)
        {
            Die();
        }
    }


    /// <summary>
    /// 피격시 간단한 이펙트
    /// </summary>
    private IEnumerator PlayerFlashCo()
    {
        Color originalColor = SpriteRenderer.color;

        SpriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        SpriteRenderer.color = originalColor;
    }

    private void Die()
    {
        Debug.Log("플레이어 사망"); 
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, data.attackRange);
    }

}
