using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TowerBuilderEditorConfiguration", menuName = "Scriptable Objects/TowerBuilderEditorConfiguration")]
public class TowerBuilderEditorConfiguration : ScriptableObject
{
    [Header("List of tower descriptors")]
    public List<TowerDescriptor> descriptors;

    [Header("Button Prefab")]
    public GameObject towerButtonPrefab;
}
