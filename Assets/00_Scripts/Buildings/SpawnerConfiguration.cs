using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "SpawnerConfiguration", menuName = "Enemies/SpawnerConfiguration")]
public class SpawnerConfiguration : ScriptableObject
{
    public List<EnemyWave> enemyWaves;
}
