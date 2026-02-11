using UnityEngine;
using UnityEngine.UI;

[System.Serializable]

[CreateAssetMenu(fileName = "HexGridConfiguration", menuName = "Scriptable Objects/HexGridConfiguration")]
public class HexGridConfiguration : ScriptableObject
{
    [Header("Hexgrid prefabs")]
    public HexCell cellPrefab;
    public Text cellLabelPrefab;
    public HexGridChunk chunkPrefab;
    
    [Header("Building Prefabs")]
    public GameObject nexusPrefab;
    public GameObject spawnerPrefab;
}
