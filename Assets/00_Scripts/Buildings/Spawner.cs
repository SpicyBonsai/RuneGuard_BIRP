using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public SpawnerConfiguration configuration;

    private float _waveTimer;

    private int _waveIndex = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _waveTimer = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (_waveTimer + configuration.inBetweenWavesDelay > Time.time) return;
        if(_waveIndex >= configuration.enemyWaves.Count) return;
        
        StartCoroutine(SpawnWave(configuration.enemyWaves[_waveIndex]));
        _waveIndex++;
        _waveTimer = Time.time;
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
