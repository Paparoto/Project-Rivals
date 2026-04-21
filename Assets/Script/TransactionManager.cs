using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class TransactionManager : MonoBehaviour
{
    // --- AJOUT : LE SINGLETON ---
    public static TransactionManager Instance;
    private BonusManager bonusManager;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }


    [Header("Lien avec le Visuel")]
    public FoodVisualizer foodVisualizer; // Gardé pour compatibilité, mais on peut faire mieux

    [Header("Modèles des Produits (Prefabs)")]
    public GameObject Apple;

    [Header("UI Argent")]
    public TMP_Text moneyTextP1;
    public TMP_Text moneyTextP2;

    public class Product
    {
        public string name;
        public string category;
        public int buyPrice;
        public int sellPrice;
        public string bonus;
        public string malus;
        public GameObject prefab;

        public Product(string name, string category, int buyPrice, int sellPrice, string bonus, string malus, GameObject prefab)
        {
            this.name = name;
            this.category = category;
            this.buyPrice = buyPrice;
            this.sellPrice = sellPrice;
            this.bonus = bonus;
            this.malus = malus;
            this.prefab = prefab;
        }
    }

    public class PlayerData
    {
        public int money = 100;
        public List<Product> inventory = new List<Product>();
        public List<int> profits = new List<int>();
    }

    public PlayerData player1 = new PlayerData();
    public PlayerData player2 = new PlayerData();

    void Start()
    {
        player1.money = 500;
        bonusManager = FindObjectOfType<BonusManager>();
        RefreshVisuals();
    }

    public bool Buy(PlayerData player, Product product)
    {
        if (player.money < product.buyPrice)
        {
            Debug.Log("Pas assez d'argent");
            return false;
        }

        player.money -= product.buyPrice;
        player.inventory.Add(product);
        
        RefreshVisuals();
        return true;
    }

    public void Sell(PlayerData player, Product product)
    {
        if (!player.inventory.Contains(product)) return;

        player.inventory.Remove(product);
        player.money += product.sellPrice;
        RefreshVisuals();
    }

    public int GetProfit(PlayerData player)
{
    int inventoryValue = 0;
    foreach (Product p in player.inventory)
        inventoryValue += p.buyPrice;

    int total = player.money + inventoryValue;
    Debug.Log($"Argent: {player.money}$ | Inventaire: {inventoryValue}$ | Total: {total}$");
    return total;
}

    private void RefreshVisuals()
    {
        // On rafraîchit TOUS les scripts de visuels dans la scène (P1 et P2)
        FoodVisualizer[] allVisuals = FindObjectsOfType<FoodVisualizer>();
        foreach (FoodVisualizer fv in allVisuals)
        {
            fv.Refresh();
        }

        if (moneyTextP1 != null) moneyTextP1.text = $"{player1.money}$";
        if (moneyTextP2 != null) moneyTextP2.text = $"{player2.money}$";
    }
}