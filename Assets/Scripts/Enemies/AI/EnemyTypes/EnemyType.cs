using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/EnemyType")]
public class EnemyType : ScriptableObject
{
    [Range(0f, 100f)]
    public float maxHealthPoint;
    public EnemyBehaveType behaveType;
    [Range(2.5f, 10f)]
    public float attackRange;
    [Range(5f, 15f)]
    public float viewRadius;
    [Range(1f, 5f)]
    public float moveSpeed;
    [Range(0.5f, 1.5f)]
    public float attackCoolDown;
    [Range(5f, 25f)]
    public float attackDamage;
}

[System.Serializable]
public enum EnemyBehaveType
{
    Melee,
    Range,
    Guard
}
