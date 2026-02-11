using System;
using UnityEngine;

[CreateAssetMenu(fileName = "TowerDescriptor", menuName = "Scriptable Objects/TowerDescriptor")]
public class TowerDescriptor : ScriptableObject
{
    [Tooltip("Name of the tower")]
    public string name;
    
    [Min(0f)]
    public float damage;

    [SerializeField] private float shotsPerSecond;
    [NonSerialized] public float delayBetweenShots;

    [Range(0,100)] [Tooltip("0 means 0% chance ")]
    public float critChance = 0;
    [Tooltip("Multiplier for the critical hit damage")]
    public float critDamageMultiplier = 2;

    [Min(2)]
    public float range;
    [NonSerialized] public float sqRange;

    public GameObject prefab;
    public int cost;
    
    private void OnValidate()
    {
        sqRange = range * range;
        delayBetweenShots = 1f / shotsPerSecond;

        if (!prefab.TryGetComponent(out BaseTower tower))
        {
            tower = prefab.AddComponent<BaseTower>();
        }
        tower.descriptor = this;
        tower.area.radius = range;
    }
}
