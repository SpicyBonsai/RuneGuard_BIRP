using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "EnemyWave", menuName = "Enemies/EnemyWave")]
public class EnemyWave : ScriptableObject
{
    public List<GameObject> enemies;
    public float inBetweenSpawnsDelay = .5f;
}
