using System;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    public HexMapDescriptor descriptor;
    
    private ActionPhase _actionPhase = ActionPhase.Building;
    private List<Spawner> _spawners = new();

    //private MapDescriptor _descriptor;
    private int _money = 100;
    private HexGrid _grid;
    
    public int Money
    {
        get => _money;
        set
        {
            _money = value;
            foreach (var moneyDisplay in moneyDisplays)
            {
                moneyDisplay.UpdateText();
            }
        }
    }

    [HideInInspector] public List<MoneyDisplay> moneyDisplays = new();
    
    public ActionPhase Phase
    {
        private set => _actionPhase = value;
        get => _actionPhase;
    }

    public enum ActionPhase
    {
        Building,
        Fighting
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void NextStage() 
    {
        if (Phase == ActionPhase.Building)
        {
            Phase = ActionPhase.Fighting;
            foreach (var spawner in _spawners)
            {
                spawner.SpawnNextWave();
            }
        }
        else
        {
            Phase = ActionPhase.Building;
        }
    }
 
    public void AddSpawner(Spawner spawner)
    {
        if(_spawners.Contains(spawner)) return;
        _spawners.Add(spawner);
    }

    public void RemoveSpawner(Spawner spawner)
    {
        if (_spawners.Contains(spawner))
        {
            _spawners.Remove(spawner);
        }
    }

    public void SetGrid(HexGrid grid)
    {
        _grid = grid;

        if (!_grid || !descriptor) return;
        _grid.LoadOnDescriptor(descriptor);
    }
}
