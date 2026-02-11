using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class TowerBuilderEditor : MonoBehaviour
{
    public HexGrid hexGrid;
    public TowerBuilderEditorConfiguration configuration;
    public Transform towersTab;
    
    private TowerDescriptor _selectedTowerDescriptor;

    private void Start()
    {
        RefreshTowerTabs();
    }

    void Update () 
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) 
        {
            HandleInput();
        }
    }

    void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(inputRay, out RaycastHit hit) || hexGrid == null) return;
        
        HexCell center = hexGrid.GetCell(hit.point);
        if (center.Type != HexCell.HexCellType.buildable || center.HasBuilding)
        {
            //TODO: add a function that says you can't build here
            Debug.LogWarning("You can't build here.");
            return;
        }
        
        if (_selectedTowerDescriptor.cost > GameController.Instance.Money)
        {
            //TODO: add a function that says you can't buy it
            Debug.LogWarning("Not enough money to buy it.");
            return;
        }
        
        GameController.Instance.Money -= _selectedTowerDescriptor.cost;
        center.AddTower(_selectedTowerDescriptor);
    }

    public void SelectTowerDescriptor(TowerDescriptor towerDescriptor)
    {
        _selectedTowerDescriptor = towerDescriptor;
    }
    
    private void OnValidate()
    {
        if(!Application.isPlaying) return;
        RefreshTowerTabs();
    }

    private void RefreshTowerTabs()
    {
        for (int i = 0; i < towersTab.childCount; i++)
        {
            Destroy(towersTab.GetChild(i).gameObject);
        }

        foreach (var descriptor in configuration.descriptors)
        {
            GameObject towerButton = Instantiate(configuration.towerButtonPrefab, towersTab);
            TowerSelectButton towerSelectButton = towerButton.GetComponent<TowerSelectButton>();
            towerSelectButton.towerDescriptor = descriptor;
            towerSelectButton.SetTowerBuilderEditor(this);
        }
    }
    
    public void NextWave()
    {
        GameController.Instance.NextStage();
    }
}
