using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HexCellData
{
    public int terrainTypeIndex;
    public int elevation;
    public HexCell.HexCellType type;
}

[CreateAssetMenu(fileName = "HexMapDescriptor", menuName = "Scriptable Objects/HexMapDescriptor")]
public class HexMapDescriptor : ScriptableObject
{
    public int cellCountX;
    public int cellCountZ;
    
    public List<HexCellData> cells = new();

    public void SetCells(HexCell[] other)
    {
        cells.Clear();
        foreach (var cell in other)
        {
            HexCellData newCell = new HexCellData
            {
                elevation = cell.Elevation,
                type = cell.Type,
                terrainTypeIndex = cell.TerrainTypeIndex
            };
            cells.Add(newCell);
        }
    }
}
