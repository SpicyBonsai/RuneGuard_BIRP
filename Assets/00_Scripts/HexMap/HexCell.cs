using UnityEngine;
using System.IO;

public class HexCell : MonoBehaviour
{
    public HexCoordinates coordinates;

    public RectTransform uiRect;

    public HexGridChunk chunk;

    [SerializeField]
    private HexCellType _type = HexCellType.path;

    private GameObject _buildingInstance;
    public bool HasBuilding => _buildingInstance;
    
    public HexCellType Type
    {
        get => _type;
        set
        {
            if(_type == value) return;
            if (Elevation == 0)
            {
                if (value is HexCellType.buildable or HexCellType.nonbuildable) value = HexCellType.path;
            }
            else
            {
                if(value is HexCellType.nexus or HexCellType.path or HexCellType.spawner) value = HexCellType.buildable;
            }
            _type = value;
            HexPathManager.Instance.RefreshPathfindingWorld();
        }
    }
    public enum HexCellType
    {
        path = 0,
        spawner = 1,
        nexus = 2,
        buildable = 3,
        nonbuildable = 4
    }
    
    void RefreshPosition()
    {
        Vector3 position = transform.localPosition;
        position.y = elevation * HexMetrics.elevationStep;
        transform.localPosition = position;

        Vector3 uiPosition = uiRect.localPosition;
        uiPosition.z = elevation * -HexMetrics.elevationStep;
        uiRect.localPosition = uiPosition;

        HexPathManager.Instance.RefreshPathfindingWorld();
    }
    public int Elevation
    {
        get
        {
            return elevation;
        }
        set
        {
            if (elevation == value)
            {
                Type = value == 0 ? HexCellType.path : HexCellType.buildable;
                return;
            }

            elevation = value;
            
            RefreshPosition();
            Refresh();
            
            Type = value == 0 ? HexCellType.path : HexCellType.buildable;
        }
    }

    int elevation;

    public int TerrainTypeIndex
    {
        get => terrainTypeIndex;
        set
        {
            if (terrainTypeIndex == value) return;
            
            terrainTypeIndex = value;
            Refresh();
        }
    }

    int terrainTypeIndex;

    public Color Color
    {
        get
        {
            return HexMetrics.colors[terrainTypeIndex];
        }
    }

    [SerializeField]
    HexCell[] neighbors;

    public HexCell GetNeighbor(HexDirection direction)
    {
        return neighbors[(int)direction];
    }
    
    public HexCell[] GetNeighbors()
    {
        return neighbors;
    }

    public void SetNeighbor(HexDirection direction, HexCell cell)
    {
        neighbors[(int)direction] = cell;
        cell.neighbors[(int)direction.Opposite()] = this;
    }

    public HexEdgeType GetEdgeType(HexDirection direction)
    {
        return HexMetrics.GetEdgeType(
            elevation, neighbors[(int)direction].elevation
        );
    }

    public HexEdgeType GetEdgeType(HexCell otherCell)
    {
        return HexMetrics.GetEdgeType(
            elevation, otherCell.elevation
        );
    }

    void Refresh()
    {
        if (chunk)
        {
            chunk.Refresh();
            for (int i = 0; i < neighbors.Length; i++)
            {
                HexCell neighbor = neighbors[i];
                if (neighbor != null && neighbor.chunk != chunk)
                {
                    neighbor.chunk.Refresh();
                }
            }
        }
    }

    public void AddBuilding(GameObject buildingPrefab)
    {
        _buildingInstance = Instantiate(buildingPrefab, transform.position, Quaternion.identity, gameObject.transform);
    }

    public void DeleteBuilding()
    {
        if(!HasBuilding) return;
        Destroy(_buildingInstance.gameObject);
    }
    
    public void Save(BinaryWriter writer)
    {
        writer.Write((byte)terrainTypeIndex);
        writer.Write((byte)elevation);
        writer.Write((byte)_type);
    }

    public void Load(BinaryReader reader)
    {
        terrainTypeIndex = reader.ReadByte();
        elevation = reader.ReadByte();
        _type = (HexCellType)reader.ReadByte();
        
        RefreshPosition();
    }

    public void AddTower(TowerDescriptor selectedTowerDescriptor)
    {
        _buildingInstance = Instantiate(selectedTowerDescriptor.prefab, transform.position, Quaternion.identity, gameObject.transform);
    }
}