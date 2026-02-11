using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEditor;

public class HexGrid : MonoBehaviour
{
    public int cellCountX = 20, cellCountZ = 15;
    private int _chunkCountX, _chunkCountZ;
    public Color[] colors;

    public HexGridConfiguration configuration;

    private HexCell[] _cells;
    
    public HexCell[] Cells
    {
        get => _cells;
    }
    HexGridChunk[] _chunks;  

    void Awake()
    {
        HexMetrics.colors = colors;
        CreateMap(cellCountX, cellCountZ);
        
        GameController.Instance.SetGrid(this);
    }

    private void Start()
    {
        HexPathManager.Instance.RefreshPathfindingWorld();
    }

    void OnEnable()
    {
        HexMetrics.colors = colors;
    }  

    public bool CreateMap(int x, int z)
    {
        if (x <= 0 || x % HexMetrics.chunkSizeX != 0 || z <= 0 || z % HexMetrics.chunkSizeZ != 0)
        {
            Debug.LogError("Unsupported map size.");
            return false;
        }

        if (_chunks != null)
        {
            for (int i = 0; i < _chunks.Length; i++)
            {
                Destroy(_chunks[i].gameObject);
            }
        }

        cellCountX = x;
        cellCountZ = z;

        _chunkCountX = cellCountX / HexMetrics.chunkSizeX;
        _chunkCountZ = cellCountZ / HexMetrics.chunkSizeZ;

        CreateChunks();
        CreateCells();
        return true;
    }

    void CreateChunks()
    {
        _chunks = new HexGridChunk[_chunkCountX * _chunkCountZ];

        for (int z = 0, i = 0; z < _chunkCountZ; z++)
        {
            for (int x = 0; x < _chunkCountX; x++)
            {
                HexGridChunk chunk = _chunks[i++] = Instantiate(configuration.chunkPrefab);
                chunk.transform.SetParent(transform);
            }
        }
    }

    void CreateCells()
    {
        _cells = new HexCell[cellCountZ * cellCountX];

        for (int z = 0, i = 0; z < cellCountZ; z++)
        {
            for (int x = 0; x < cellCountX; x++)
            {
                CreateCell(x, z, i++);
            }
        }
    }

    void CreateCell(int x, int z, int i)
    {
        Vector3 position;
        position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
        position.y = 0f;
        position.z = z * (HexMetrics.outerRadius * 1.5f);

        HexCell cell = _cells[i] = Instantiate<HexCell>(configuration.cellPrefab);
        cell.transform.localPosition = position;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);

        if (x > 0)
        {
            cell.SetNeighbor(HexDirection.W, _cells[i - 1]);
        }
        if (z > 0)
        {
            if ((z & 1) == 0)
            {
                cell.SetNeighbor(HexDirection.SE, _cells[i - cellCountX]);
                if (x > 0)
                {
                    cell.SetNeighbor(HexDirection.SW, _cells[i - cellCountX - 1]);
                }
            }
            else
            {
                cell.SetNeighbor(HexDirection.SW, _cells[i - cellCountX]);
                if (x < cellCountX - 1)
                {
                    cell.SetNeighbor(HexDirection.SE, _cells[i - cellCountX + 1]);
                }
            }
        }

        Text label = Instantiate<Text>(configuration.cellLabelPrefab);
        label.rectTransform.anchoredPosition =
            new Vector2(position.x, position.z);
        label.text = cell.coordinates.ToStringOnSeparateLines();
        cell.uiRect = label.rectTransform;

        AddCellToChunk(x, z, cell);
    }

    void AddCellToChunk(int x, int z, HexCell cell)
    {
        int chunkX = x / HexMetrics.chunkSizeX;
        int chunkZ = z / HexMetrics.chunkSizeZ;
        HexGridChunk chunk = _chunks[chunkX + chunkZ * _chunkCountX];

        int localX = x - chunkX * HexMetrics.chunkSizeX;
        int localZ = z - chunkZ * HexMetrics.chunkSizeZ;
        chunk.AddCell(localX + localZ * HexMetrics.chunkSizeX, cell);
    }

    public HexCell GetCell(Vector3 position)
    {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        int index = coordinates.X + coordinates.Z * cellCountX + coordinates.Z / 2;
        return _cells[index];
    }

    public HexCell GetCell(HexCoordinates coordinates)
    {
        int z = coordinates.Z;
        if (z < 0 || z >= cellCountZ)
        {
            return null;
        }
        int x = coordinates.X + z / 2;
        if (x < 0 || x >= cellCountX)
        {
            return null;
        }
        return _cells[x + z * cellCountX];
    }

    public void ShowUI(bool visible)
    {
        for (int i = 0; i < _chunks.Length; i++)
        {
            _chunks[i].ShowUI(visible);
        }
    }

    public void Save(BinaryWriter writer, string fileName = "unnamed")
    {
        writer.Write(cellCountX);
        writer.Write(cellCountZ);
        for (int i = 0; i < _cells.Length; i++)
        {
            _cells[i].Save(writer);
        }

        SaveOnDescriptor(fileName);
    }

    public void Load(BinaryReader reader, int header)
    {
        int x = 20, z = 15;
        if (header >= 1)
        {
            x = reader.ReadInt32();
            z = reader.ReadInt32();
        }
        if (x != cellCountX || z != cellCountZ)
        {
            if (!CreateMap(x, z))
            {
                return;
            }
        }

        for (int i = 0; i < _cells.Length; i++)
        {
            _cells[i].Load(reader);
            if(_cells[i].Type == HexCell.HexCellType.nexus) AddNexus(_cells[i]);
            if(_cells[i].Type == HexCell.HexCellType.spawner) AddSpawner(_cells[i]);
        }
        for (int i = 0; i < _chunks.Length; i++)
        {
            _chunks[i].Refresh();
        }
    }

    public void SaveOnDescriptor(string fileName)
    {
        HexMapDescriptor mapDescriptor = new();
        mapDescriptor.cellCountX = cellCountX;
        mapDescriptor.cellCountZ = cellCountZ;
        mapDescriptor.SetCells(_cells);
        
        AssetDatabase.CreateAsset (mapDescriptor, "Assets/TestMaps/"+ fileName.Replace(" ", "")+".asset");
        AssetDatabase.SaveAssets ();
        EditorUtility.FocusProjectWindow ();
        Selection.activeObject = mapDescriptor;
    }

    public void LoadOnDescriptor(HexMapDescriptor mapDescriptor)
    {
        int x = mapDescriptor.cellCountX;
        int z = mapDescriptor.cellCountZ;
        
        if (!CreateMap(x, z))
        {
            return;
        }
        
        for (int i = 0; i < _cells.Length; i++)
        {
            _cells[i].TerrainTypeIndex = mapDescriptor.cells[i].terrainTypeIndex;
            _cells[i].Elevation = mapDescriptor.cells[i].elevation;
            _cells[i].Type = mapDescriptor.cells[i].type;
            
            if(_cells[i].Type == HexCell.HexCellType.nexus) AddNexus(_cells[i]);
            if(_cells[i].Type == HexCell.HexCellType.spawner) AddSpawner(_cells[i]);
        }
        for (int i = 0; i < _chunks.Length; i++)
        {
            _chunks[i].Refresh();
        }
    }
    
    public void AddSpawner(HexCell h)
    {
        if(h.HasBuilding) return;
        h.AddBuilding(configuration.spawnerPrefab);
    }

    public void DeleteSpawner(HexCell h)
    {
        h.DeleteBuilding();
    }
    
    public void AddNexus(HexCell h)
    {
        if(h.HasBuilding) return;
        h.AddBuilding(configuration.nexusPrefab);
    }
    
    public void DeleteNexus(HexCell h)
    {
        h.DeleteBuilding();
    }
}