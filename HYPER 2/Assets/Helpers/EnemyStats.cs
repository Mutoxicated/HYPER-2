using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Stats", menuName = "Mutox/Enemy Stats")]
public class EnemyStats : ScriptableObject
{
    [Range(1, 10)] public int difficulty = 1;
    [Range(1, 3)] public int verticality = 1;
}
