using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ProductUIItem : MonoBehaviour
{
    public TMP_Text categoryText, nameText, priceText, bonusText, malusText, stockText;
    public Button buyButton;
    public GameObject selectionFrame; // Glisse l'objet SelectionFrame ici

    private TransactionManager.Product data;
    private TransactionManager.PlayerData owner;
    private TransactionManager manager;

    public void Setup(TransactionManager.Product p, TransactionManager.PlayerData plr, TransactionManager tm)
    {
        data = p; owner = plr; manager = tm;
        categoryText.text = $"[{p.category}]";
        nameText.text = p.name;
        priceText.text = $"{p.buyPrice}/{p.sellPrice}";
        bonusText.text = p.bonus;
        malusText.text = p.malus;
        UpdateStock();
    }

    public void UpdateStock()
    {
        int count = owner.inventory.FindAll(x => x.name == data.name).Count;
        stockText.text = "Stk:" + count;
    }

    public void SetSelected(bool isSelected)
    {
        if (selectionFrame != null) selectionFrame.SetActive(isSelected);
    }
}