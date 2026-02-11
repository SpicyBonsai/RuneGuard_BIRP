using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public SpawnerConfiguration configuration;
    
    private int _waveIndex = -1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameController.Instance.AddSpawner(this);
    }

    private void OnDestroy()
    {
        GameController.Instance.RemoveSpawner(this);
    }

    public void SpawnNextWave()
    {
        if(_waveIndex + 1 >= configuration.enemyWaves.Count) return;
        
        _waveIndex++;
        StartCoroutine(SpawnWave(configuration.enemyWaves[_waveIndex]));
    }
    
    IEnumerator SpawnWave(EnemyWave enemyWave)
    {
        foreach (var enemy in enemyWave.enemies)
        {
            Instantiate(enemy.gameObject, transform.position, Quaternion.identity);
            yield return new WaitForSeconds(enemyWave.inBetweenSpawnsDelay);
        }
    }
}
