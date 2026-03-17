using UnityEngine;

public class EnemyData
{
    public int EnemyId;
    public string Name;
    public float hp;
    public float atk;
    public float def;
    public float moveSpeed;
    public float goldReward;
}

public enum EnemyType
{
    // CSVÀÇ ID ¼ıÀÚ¿Í ¶È°°ÀÌ ¸ÂÃã

    golem = 1,        // °ñ·½ 
    PaperMonster = 2, // ½ºÄÌ·¹Åæ
}