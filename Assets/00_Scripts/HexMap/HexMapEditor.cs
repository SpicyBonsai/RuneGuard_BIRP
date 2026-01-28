using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.IO;
using TMPro;

public class HexMapEditor : MonoBehaviour {

    bool applyElevation = true;
    bool applyFlood = true;
    bool applyColor = true;
    int brushSize;
    int activeTerrainTypeIndex;

    string mapName;

    public HexGrid hexGrid;

    public int activeElevation;
    public HexCell.HexCellType activeType;
    private List<HexCell> _floodedCells = new();

    public TMP_Dropdown dropdown;
    
    private void Start()
    {
        dropdown.options = new();
        dropdown.options.Clear();
        foreach (var optionData in Enum.GetValues(typeof(HexCell.HexCellType)))
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData(optionData.ToString()));
        }
    }

    void Update () 
	{
		if (
			Input.GetMouseButton(0) &&
			!EventSystem.current.IsPointerOverGameObject()
		) 
		{
			HandleInput();
		}
	}

	void HandleInput () 
    {
		Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit) && hexGrid != null)
        {
            if (applyFlood)
            {
                HexCell center = hexGrid.GetCell(hit.point);
                int originalElevation = center.Elevation;
                Color originalColor = center.Color;
                EditCell(center);

                if (applyElevation)
                {
                    _floodedCells.Clear();
                    _floodedCells.Add(center);
                    EditFloodElevation(center, originalElevation);
                }
                
                if (applyColor)
                {
                    _floodedCells.Clear();
                    _floodedCells.Add(center);
                    EditFloodColor(center, originalColor);
                }
            }
            else
            {
                EditCells(hexGrid.GetCell(hit.point));
            }
        }
    }

    void EditCells(HexCell center) 
    {
        int centerX = center.coordinates.X;
        int centerZ = center.coordinates.Z;

        for (int r = 0, z = centerZ - brushSize; z <= centerZ; z++, r++)
        {
            for (int x = centerX - r; x <= centerX + brushSize; x++)
            {
                EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
            }
        }
        for (int r = 0, z = centerZ + brushSize; z > centerZ; z--, r++)
        {
            for (int x = centerX - brushSize; x <= centerX + r; x++)
            {
                EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
            }
        }
    }


    void EditFloodElevation(HexCell center, int originalElevation)
    {
        foreach (var neighbour in center.GetNeighbors())
        {
            if(neighbour == null) continue;
            if(_floodedCells.Contains(neighbour)) continue;

            if (applyElevation && neighbour.Elevation != originalElevation) continue;
            EditCell(neighbour);
            _floodedCells.Add(neighbour);
            EditFloodElevation(neighbour, originalElevation);
        }
    }

    void EditFloodColor(HexCell center, Color originalColor)
    {
        foreach (var neighbour in center.GetNeighbors())
        {
            if(neighbour == null) continue;
            if(_floodedCells.Contains(neighbour)) continue;

            if (applyElevation && neighbour.Color != originalColor) continue;
            EditCell(neighbour);
            _floodedCells.Add(neighbour);
            EditFloodColor(neighbour, originalColor);
        }
    }
    
    void EditCell(HexCell cell)
    {
        if (!cell) return;
        
        if (applyColor)
        {
            cell.TerrainTypeIndex = activeTerrainTypeIndex;
        }
        if (applyElevation)
        {
            cell.Elevation = activeElevation;
        }

        if (cell.Type != activeType)
        {
            if(activeType == HexCell.HexCellType.nexus)
            {
                hexGrid.AddNexus(cell);
            }
            if (cell.Type == HexCell.HexCellType.nexus)
            {
                hexGrid.DeleteNexus(cell);
            }
            
            if (activeType == HexCell.HexCellType.spawner)
            {
                hexGrid.AddSpawner(cell);
            }   
            if (cell.Type == HexCell.HexCellType.spawner)
            {
                hexGrid.DeleteSpawner(cell);
            }
        }
        cell.Type = activeType;
        
    }

    public void SetTerrainTypeIndex(int index)
    {
        activeTerrainTypeIndex = index;
    }

    public void SetElevation(float elevation)
    {
        activeElevation = (int)elevation;
    }
    
    public void SetHexCellType(int value)
    {
        activeType = (HexCell.HexCellType)value;
    }

    public void SetApplyElevation(bool toggle)
    {
        applyElevation = toggle;
    }
    
    public void SetApplyFlood(bool toggle)
    {
        applyFlood = toggle;
    }

    public void SetApplyColor(bool toggle)
    {
        applyColor = toggle;
    }

    public void SetBrushSize(float size)
    {
        brushSize = (int)size;
    }

    public void ShowUI(bool visible)
    {
        hexGrid.ShowUI(visible);
    }

    public void SetMapName(string name)
    {
        mapName = name;
    }

    public void Save()
    {
        if (mapName == null || mapName == "")
        {
            mapName = "unnamed";
        }
        string path = Path.Combine(Application.persistentDataPath, mapName + ".map");
        using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create)))
        {
            writer.Write(1);
            hexGrid.Save(writer);
        }
        Debug.Log("Saved " + mapName + " under " + path);
    }

    public void Load()
    {
        if (mapName == null || mapName == "")
        {
            mapName = "unnamed";
        }
        string path = Path.Combine(Application.persistentDataPath, mapName + ".map");
        using (BinaryReader reader = new BinaryReader(File.OpenRead(path)))
        {
            int header = reader.ReadInt32();
            if (header <= 1)
            {
                hexGrid.Load(reader, header);
                HexMapCamera.ValidatePosition();
            }
            else
            {
                Debug.LogWarning("Unknown map format " + header);
            }
        }
        Debug.Log("Loaded " + mapName + " from " + path);
    }
}