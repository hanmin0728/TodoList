public sealed class EnemyData
{
    public int EnemyId;
    public string Name;
    public float Hp;
    public float Atk;
    public float Def;
    public float MoveSpeed;
    public float AttackRange;
    public float AttackDelay;
    public long GoldReward;
}

public enum EnemyType
{
    golem = 1,
}
