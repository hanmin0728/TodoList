using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Scriptable Objects/PlayerData")]
public class PlayerData : ScriptableObject
{
    [Header("이동 속도")]
    public float scrollSpeed = 5.0f;

    [Header("전투 능력치")]
    public float hp;
    public float attackRange = 1.5f;
    public float attackDamage = 10f;
    public float attackDelay = 1.0f;
    public float knockBackForce = 3f;

    [Header("애니메이션 파라미터")]
    public string attacAnimationParam = "PlayerAttack";
}
