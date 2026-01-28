using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyDescriptor descriptor;
    private HexPathManager.HexCellPath _toBeFollowedPath;
    private int _pathIndex = 1;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _toBeFollowedPath = HexPathManager.Instance.GetRandomPath();
        transform.position = _toBeFollowedPath.path[0].transform.position;
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
}
