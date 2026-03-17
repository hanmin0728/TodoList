using UnityEngine;
using UnityEngine.Rendering;

public abstract class EnemyBase : MonoBehaviour, IDamageable
{
    public StateMachine<EnemyBase> StateMachine { get; protected set; }

    #region 적에게 존재하는 상태 패턴
    public EnemyChaseState ChaseState { get; protected set; }
    public EnemyAttackState AttackState { get; protected set; }
    public EnemyHitState HitState { get; protected set; }
    public EnemyDieState DieState { get; protected set; }
    #endregion

    public EnemyData data;

    protected float currentHp;
    protected bool IsDead = false;

    public Animator Anim { get; protected set; }
    public void Awake()
    {
        Anim = GetComponent<Animator>();

        StateMachine = new StateMachine<EnemyBase>();

        ChaseState = new EnemyChaseState(this, StateMachine);
        AttackState = new EnemyAttackState(this, StateMachine);
        HitState = new EnemyHitState(this, StateMachine);
        DieState = new EnemyDieState(this, StateMachine);
    }
    public void Init(EnemyData newData)
    {
        data = newData;
        currentHp = data.hp; // 최대 체력 정보를 가져와 현재 체력 초기화

        StateMachine.Initialize(ChaseState);
    }
    protected virtual void Update()
    {
        if (StateMachine.CurrentState != null)
            StateMachine.CurrentState.Update();
    }

    public virtual void OnDamage(float damage)
    {
        if (IsDead) return;

        currentHp -= damage;

        if (currentHp <= 0)
        {
            IsDead = true;
            StateMachine.ChangeState(DieState); // 죽음 상태로 전환 
            //리워드 지급 및 애니메이션 실행은 DieState에서 처리
        }
        else
        {
            StateMachine.ChangeState(HitState);
            //피격 애니메이션 실행은 HitState에서 처리
        }
    }

    /// <summary>
    /// 자식 클래스에서 본인만의 공격 방식 구현
    /// </summary>
    public abstract void PerformAttack(); 
}
