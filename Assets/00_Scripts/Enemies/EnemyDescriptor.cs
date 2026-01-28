using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDescriptor", menuName = "Enemies/EnemyDescriptor")]
public class EnemyDescriptor : ScriptableObject
{
    [Min(0f)]
    public float speed;
    [Min(.1f)]
    public float sqWalkRadius;

    private void OnValidate()
    {
        if (speed < 0) speed = 0f;
        if (sqWalkRadius < .1f) sqWalkRadius = .1f;
    }
}
