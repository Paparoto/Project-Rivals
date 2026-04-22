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

    // Détermine quel joueur
    bool isP1 = (player == player1);

    // Applique le bonus selon le produit
    switch (product.name)
    {
        case "Cookies":
            if (isP1) {bonusManager.P1speedBonus += 0.20f ; bonusManager.P1patienceBonus -= 0.1f; }
            else {bonusManager.P2speedBonus += 0.20f; bonusManager.P1patienceBonus -= 0.1f; }
            break;
        case "Thon":
            if (isP1) { bonusManager.P1moneyBonus += 0.10f; bonusManager.P1clientBonus += 0.10f; }
            else { bonusManager.P2moneyBonus += 0.10f; bonusManager.P2clientBonus += 0.10f; }
            break;
        case "Saumon":
            if (isP1) bonusManager.P1moneyBonus += 0.05f;
            else bonusManager.P2moneyBonus += 0.05f;
            break;
        case "Fromage bleu":
            if (isP1) bonusManager.P1clientBonus += 0.15f;
            else bonusManager.P2clientBonus += 0.15f;
            break;
        case "Emmental":
            if (isP1) bonusManager.P1moneyBonus -= 0.05f;
            else bonusManager.P2moneyBonus -= 0.05f;
            break;
        case "Entrecote":
            if (isP1) {bonusManager.P1speedBonus -= 0.05f; bonusManager.P1clientBonus -= 0.1f;}
            else {bonusManager.P2speedBonus -= 0.05f; bonusManager.P2clientBonus -= 0.1f;}
            break;
        case "Boeuf":
            if (isP1) bonusManager.P1speedBonus -= 0.08f;
            else bonusManager.P2speedBonus -= 0.08f;
            break;
        case "pomme":
            if (isP1) bonusManager.P1speedBonus += 0.10f;
            else bonusManager.P2speedBonus += 0.10f;
            break;
        case "Melon":
            if (isP1) bonusManager.P1speedBonus += 0.08f;
            else bonusManager.P2speedBonus += 0.08f;
            break;
        case "Gateau":
            if (isP1){ bonusManager.P1clientBonus -= 0.10f; bonusManager.P1patienceBonus -= 0.05f; }
            else {bonusManager.P2clientBonus -= 0.10f; bonusManager.P2patienceBonus -= 0.05f; }
            break;
        case "Poivron":
            if (isP1) bonusManager.P1patienceBonus += 0.1f;
            else bonusManager.P2patienceBonus += 0.1f;
            break;
    }

    RefreshVisuals();
    return true;
}

public void Sell(PlayerData player, Product product)
{
    if (SoundManager.Instance != null)
        SoundManager.Instance.PlaySaleSound(); 

    if (!player.inventory.Contains(product)) return;

    player.inventory.Remove(product);

    bool isP1 = (player == player1);
    float moneyBonus = isP1 ? bonusManager.P1moneyBonus : bonusManager.P2moneyBonus;

    player.money += Mathf.RoundToInt(product.sellPrice * moneyBonus);

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