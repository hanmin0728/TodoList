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

        return hit != null; 
    }

}
