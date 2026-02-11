using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BaseTower : MonoBehaviour
{
    public TowerDescriptor descriptor;

    private List<Enemy> _enemies = new();
    private float _timer = float.MinValue;

    public SphereCollider area;

    public LineRenderer laser;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _enemies.Clear();
        area = GetComponent<SphereCollider>();
        laser.gameObject.SetActive(false);
    }

    private Enemy FindClosestEnemy()
    {
        Enemy closestEnemyToNexus = null;
        float closestDistanceToNexus = float.MaxValue;
        foreach (var enemy in _enemies)
        {
            float distanceToNexus = enemy.DistanceToNexus();
            if(closestDistanceToNexus < distanceToNexus) continue;

            closestDistanceToNexus = distanceToNexus;
            closestEnemyToNexus = enemy;
        }
        
        return closestEnemyToNexus;
    }
    
    // Update is called once per frame
    void Update()
    {
        if(_enemies.Count == 0) return;
        if(_timer + descriptor.delayBetweenShots > Time.time) return;

        Enemy enemy = FindClosestEnemy();
        if(enemy == null) return;

        DealDamage(enemy);
        StartCoroutine(ShootLaser(descriptor.delayBetweenShots * .1f, enemy.transform.position));
        _timer = Time.time;
    }

    private IEnumerator ShootLaser(float time, Vector3 enemyPosition)
    {
        laser.gameObject.SetActive(true);
        laser.SetPositions(new []{laser.gameObject.transform.position, enemyPosition});
        yield return new WaitForSeconds(time);
        laser.gameObject.SetActive(false);
    }

    protected virtual void DealDamage(Enemy enemy)
    {
        float damage = descriptor.damage;

        if (descriptor.critChance > 0)
        {
            float chance = Random.Range(0f, 100f);
            if (chance <= descriptor.critChance)
            {
                damage *= descriptor.critDamageMultiplier;
            }
        }
        
        enemy.TakeDamage(damage);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if(!other.TryGetComponent(out Enemy enemy)) return;
        if(_enemies.Contains(enemy)) return;
        _enemies.Add(enemy);
    }

    private void OnTriggerExit(Collider other)
    {
        if(!other.TryGetComponent(out Enemy enemy)) return;
        if(!_enemies.Contains(enemy)) return;
        _enemies.Remove(enemy);
    }
}
