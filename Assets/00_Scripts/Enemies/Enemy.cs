using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyDescriptor descriptor;
    private HexPathManager.HexCellPath _toBeFollowedPath;
    private int _pathIndex = 1;

    private float _currentHealth;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _toBeFollowedPath = HexPathManager.Instance.GetRandomPath();
        transform.position = _toBeFollowedPath.path[0].transform.position;

        _currentHealth = descriptor.health;
    }

    // Update is called once per frame
    void Update()
    {
        if ((transform.position - _toBeFollowedPath.path[_pathIndex].transform.position).sqrMagnitude < descriptor.sqWalkRadius)
        {
            if(_pathIndex + 1 < _toBeFollowedPath.path.Count)_pathIndex++;
        }
        else
        {
            Vector3 direction = (_toBeFollowedPath.path[_pathIndex].transform.position - transform.position).normalized;
            transform.position += direction * (descriptor.speed* Time.deltaTime);
        }
    }

    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;
        
        //TODO: damage number particle
        //Instantiate/toggle prefab that has vfx and text

        if(_currentHealth > 0) return;
        Destroy(gameObject);
    }

    public float DistanceToNexus()
    {
        return _toBeFollowedPath.path.Count - _pathIndex;
    }
}
