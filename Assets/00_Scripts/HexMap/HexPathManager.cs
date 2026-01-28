using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(HexGrid))]
public class HexPathManager : MonoBehaviour
{
    private HexGrid _hexGrid;
    private List<HexCell> _walkableCells = new();
    private List<HexCell> _spawnCells = new();
    private List<HexCell> _nexusCells = new();

    public static HexPathManager Instance;

    private static bool _isRunning = false;

    public int displayedPathIndex = 0;

    [SerializeField]
    private List<HexCellPath> possiblePaths = new();
    [Serializable]
    public class HexCellPath
    {
        public List<HexCell> path = new();
        public bool failed = false;

        public HexCellPath()
        {
        }

        public HexCellPath(HexCellPath other)
        {
            failed = other.failed;
            path.AddRange(other.path);
        }
        
        public HexCellPath(HexCellPath other, HexCell connectedHexCell)
        {
            failed = other.failed;
            path.AddRange(other.path);
            path.Add(connectedHexCell);
        }
    }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _hexGrid = GetComponent<HexGrid>();
        }
        else
        {
            Debug.Log("There is a second hex path manager that shouldn't be there!", gameObject);
        }
    }

    public void RefreshPathfindingWorld()
    {
        if (_isRunning)
        {
            StopAllCoroutines();
            _isRunning = false;
        }

        StartCoroutine(BuildPathfindingWorld());
    }

    private List<HexCell> _walkedHexCells = new();
    private IEnumerator BuildPathfindingWorld()
    {
        _isRunning = true;
        _walkableCells.Clear();
        _walkedHexCells.Clear();
        _spawnCells.Clear();
        _nexusCells.Clear();
        foreach (var h in _hexGrid.Cells)
        {
            if(h == null || h.Elevation != 0) continue;
            if(h.Type == HexCell.HexCellType.spawner) _spawnCells.Add(h);
            if(h.Type == HexCell.HexCellType.nexus) _nexusCells.Add(h);
            _walkableCells.Add(h);
        }
        
        possiblePaths.Clear();
        
        foreach (var hexCell in _spawnCells)
        {
            _walkedHexCells.Clear();
            _walkedHexCells.Add(hexCell);
            HexCellPath newPath = new HexCellPath();
            newPath.path.Add(hexCell);
            HexCell formerNode = hexCell;
            possiblePaths.AddRange(CheckNeighbors(formerNode, newPath));
        }
        
        yield return null;
        _isRunning = false;
    }

    
    private List<HexCellPath> CheckNeighbors(HexCell formerNode, HexCellPath formerPath)
    {
        List<HexCellPath> paths = new();
        List<HexCell> tempPossibleNeighbourConnections = new();
        foreach (var neighbour in formerNode.GetNeighbors())
        {
            if(!neighbour) continue;
            if(!_walkableCells.Contains(neighbour)) continue;
            if(_walkedHexCells.Contains(neighbour)) continue;
                    
            if (neighbour.Type == HexCell.HexCellType.nexus)
            {
                HexCellPath neighbourPath = new HexCellPath(formerPath,neighbour);
                paths.Add(neighbourPath);
                continue;
            }
            
            tempPossibleNeighbourConnections.Add(neighbour);
            _walkedHexCells.Add(neighbour);
        }

        if (tempPossibleNeighbourConnections.Count == 0 && paths.Count == 0) formerPath.failed = true;
        
        foreach (var neighbourConnection in tempPossibleNeighbourConnections)
        {
            paths.AddRange(CheckNeighbors(neighbourConnection, new HexCellPath(formerPath,neighbourConnection)));
        }
        
        return paths;
    }
    
    public void OnDrawGizmosSelected()
    {
        for (int i = 0; i < _walkableCells.Count; i++)
        {
            switch (_walkableCells[i].Type)
            {
                default:
                case HexCell.HexCellType.path:
                    Gizmos.color = Color.grey;
                    break;
                case HexCell.HexCellType.spawner:
                    Gizmos.color = Color.red;
                    break;
                case HexCell.HexCellType.nexus:
                    Gizmos.color = Color.green;
                    break;
                case HexCell.HexCellType.buildable:
                    Gizmos.color = Color.blue;
                    break;
                case HexCell.HexCellType.nonbuildable:
                    Gizmos.color = Color.black;
                    break;
            }
            Gizmos.DrawWireSphere(_walkableCells[i].gameObject.transform.position, .5f);
        }
        
        if(possiblePaths.Count == 0)return;
        var displayedPath = possiblePaths[displayedPathIndex].path;
        for (int i = 1; i < displayedPath.Count; i++)
        {
            Gizmos.DrawLine(displayedPath[i-1].transform.position, displayedPath[i].transform.position);
        }
    }

    private void OnValidate()
    {
        if (displayedPathIndex <= 0) displayedPathIndex = 0;
        else if (displayedPathIndex >= possiblePaths.Count) displayedPathIndex = possiblePaths.Count - 1;
    }

    public HexCellPath GetRandomPath()
    {
        return possiblePaths[Random.Range(0, possiblePaths.Count)];
    }
}
