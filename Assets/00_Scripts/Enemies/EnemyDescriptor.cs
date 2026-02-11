using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDescriptor", menuName = "Enemies/EnemyDescriptor")]
public class EnemyDescriptor : ScriptableObject
{
    [Min(0f)]
    public float speed;
    [Min(.1f)]
    public float sqWalkRadius;

    [Min(1f)]
    public float health;
}
