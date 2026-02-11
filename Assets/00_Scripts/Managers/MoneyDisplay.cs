using TMPro;
using UnityEngine;

public class MoneyDisplay : MonoBehaviour
{
    private TMP_Text _text;
    
    private void Start()
    {
        GameController.Instance.moneyDisplays.Add(this);
        _text = GetComponent<TMP_Text>();
        UpdateText();
    }

    private void OnDestroy()
    {
        GameController.Instance.moneyDisplays.Remove(this);
    }

    public void UpdateText()
    {
        _text.text = "Gold: " + GameController.Instance.Money;
    }
}
