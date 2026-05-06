using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Scriptable Objects/PlayerData")]
public sealed class PlayerData : ScriptableObject
{
    [Header("Movement")]
    [SerializeField] private float scrollSpeed = 5.0f;

    [Header("Combat")]
    [SerializeField] private float hp = 100f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackDelay = 1.0f;
    [SerializeField] private float knockBackForce = 3f;

    [Header("Animation")]
    [SerializeField] private string attackAnimationParam = "PlayerAttack";
    [SerializeField] private string moveAnimationParam = "PlayerMove";

    public float ScrollSpeed => scrollSpeed;
    public float Hp => hp;
    public float AttackRange => attackRange;
    public float AttackDamage => attackDamage;
    public float AttackDelay => attackDelay;
    public float KnockBackForce => knockBackForce;
    public string AttackAnimationParam => attackAnimationParam;
    public string MoveAnimationParam => moveAnimationParam;
}
