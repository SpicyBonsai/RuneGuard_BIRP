using UnityEngine;

public class TowerSelectButton : MonoBehaviour
{
    public TowerDescriptor towerDescriptor;

    private TowerBuilderEditor _towerBuilderEditor;
    public void SetTowerBuilderEditor(TowerBuilderEditor towerBuilderEditor)
    {
        _towerBuilderEditor = towerBuilderEditor;
    }

    public void SelectTowerDescriptor()
    {
        _towerBuilderEditor.SelectTowerDescriptor(towerDescriptor);
    }
}
