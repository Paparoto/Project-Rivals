using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ProductUIItem : MonoBehaviour
{
    public TMP_Text categoryText;
    public TMP_Text nameText;
    public TMP_Text priceText;
    public TMP_Text bonusText;
    public TMP_Text malusText;
    public TMP_Text stockText;
    public Button buyButton;

    private TransactionManager.Product data;
    private TransactionManager.PlayerData owner;
    private TransactionManager manager;

    public void Setup(TransactionManager.Product product, TransactionManager.PlayerData player, TransactionManager tm)
    {
        data = product;
        owner = player;
        manager = tm;

        categoryText.text = product.category;
        nameText.text = product.name;
        priceText.text = $"B: {product.buyPrice} / S: {product.sellPrice}";
        bonusText.text = product.bonus;
        malusText.text = product.malus;

        buyButton.onClick.AddListener(BuyItem);
        UpdateStock();
    }

    void BuyItem()
    {
        if (manager.Buy(owner, data))
        {
            UpdateStock();
        }
    }

    public void UpdateStock()
    {
        // Counts how many of this product name are in the player's inventory list
        int count = owner.inventory.FindAll(p => p.name == data.name).Count;
        stockText.text = "Stock: " + count;
    }
}