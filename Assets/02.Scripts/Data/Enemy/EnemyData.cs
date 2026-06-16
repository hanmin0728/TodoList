using System.Buffers.Text;

public sealed class EnemyData
{
    public int EnemyId;
    public string Name;
    public float Hp;
    public float HpGrow;
    public float Atk;
    public float AtkGrow;
    public float MoveSpeed;
    public float AttackRange;
    public float AttackDelay;
    public long GoldReward;
    public int VariationMin;
    public int VariationMax;
    public bool IsBoss;

    public float GetHpAtStage(int stage)
    {
        return Hp * (1f + (HpGrow * stage));
    }

    public float GetAtkAtStage(int stage)
    {
        return Atk * (1f + (AtkGrow * stage));
    }
    public string GetRandomLabel()
    {
        if (VariationMax <= 0) return null;

        int randomIndex = UnityEngine.Random.Range(VariationMin, VariationMax + 1);
        return randomIndex.ToString("D4");
    }
}

public enum EnemyType
{
    Slime = 1,
    Mushman = 2,
    Larva = 3,
    Spider = 4,
    Beatle = 5,
    Bat = 6,
    Crab = 7,
    Fishman = 8,
    HermitCrab = 9,
    Hog = 10,
    Wolf = 11,
    Goblin = 900,
    Golem = 901,
    Ogre = 902,
}
