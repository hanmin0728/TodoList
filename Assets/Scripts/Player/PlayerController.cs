using UnityEngine;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour
{
    public StateMachine<PlayerController> StateMachine { get; private set; }

    public PlayerMoveState MoveState { get; private set; }
    public PlayerAttackState AttackState { get; private set; }

    [Header("플레이어 기본 데이터 설정")]
    public PlayerData data;

    public Animator Anim { get; private set; }
    [SerializeField] private LayerMask enemyLayer;

    public int AnimWalkHash { get; private set; }
    public int AnimAttackHash { get; private set; }


    private void Awake()
    {

        StateMachine = new StateMachine<PlayerController>();

        MoveState = new PlayerMoveState(this, StateMachine);
        AttackState = new PlayerAttackState(this, StateMachine);

        Anim = GetComponent<Animator>();

        GameManager.Instance.RegisterPlayer(this);

        AnimWalkHash = Animator.StringToHash(data.walkBoolParam);
        AnimAttackHash = Animator.StringToHash(data.attacTriggerParam);
    }

    private void Start()
    {
        StateMachine.Initialize(MoveState);
    }

    private void Update()
    {
        StateMachine.CurrentState.Update();
    }

    /// <summary>
    /// 플레이어 앞에 적이 있는지 체크 후 bool 값 반환
    /// </summary>
    /// <returns></returns>
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
    /// 플레이어 공격 애니메이션 끝났을시 적 있는지 체크 후 플레이어 상태 변환
    /// </summary>
    public void OnAttackSequenceFinished()
    {
        if (CheckEnemyInRange())
        {
            Anim.SetTrigger(AnimAttackHash);
        }
        else
        {
            StateMachine.ChangeState(MoveState);
        }
    }

    public void OnAttackHit()
    {
        Vector2 checkPos = (Vector2)transform.position + new Vector2(0.5f, 0);

        // 범위 내의 모든 적 감지 
        Collider2D[] hits = Physics2D.OverlapCircleAll(checkPos, data.attackRange, enemyLayer);

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out IDamageable enemy))
            {
                enemy.OnDamage(data.attackDamage, data.knockBackForce);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, data.attackRange);
    }

}
