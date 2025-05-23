using UnityEngine;

public enum EnemyType {
    COMMON,
    BOSS,
    UNCOMMON
}

[CreateAssetMenu(fileName = "Enemy Stats", menuName = "Mutox/Enemy Stats")]
public class EnemyStats : ScriptableObject
{
    [Range(1, 10)] public int difficulty = 1;
    [Range(2, 8)] public int verticality = 1;
    public EnemyType @type = EnemyType.COMMON;
    public int startChance = 50;
}
