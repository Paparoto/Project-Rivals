using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
public class TransactionManager : MonoBehaviour
{
    [Header("Lien avec le Visuel")]
    // Glisse l'objet "Food" ici dans l'inspecteur
    public FoodVisualizer foodVisualizer; 

    [Header("Modèles des Produits (Prefabs)")]
    public GameObject Apple;
    // Ajoute d'autres GameObjects ici si besoin (ex: public GameObject Bread;)
    [Header("UI Argent")]
    public TMP_Text moneyTextP1;
    public TMP_Text moneyTextP2;

    // ======================
    // 📦 CLASSE PRODUCT
    // ======================
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

    // ======================
    // 👤 CLASSE PLAYER
    // ======================
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
       Product apple1 = new Product("Pomme", "Fruit", 2, 5, "HP", "Aucun", Apple);
    player1.inventory.Add(apple1);
    player1.money = 500;

    RefreshVisuals();
    }

    // ======================
    // 🛒 SYSTÈME D'ACHAT
    // ======================
    public bool Buy(PlayerData player, Product product)
    {
        if (player.money < product.buyPrice)
        {
            Debug.Log("Pas assez d'argent");
            return false;
        }

        player.money -= product.buyPrice;
        player.inventory.Add(product);

        Debug.Log($"Acheté : {product.name}. Argent restant : {player.money}");
        
        RefreshVisuals(); // Mise à jour visuelle
        return true;
    }

    // ======================
    // 💸 SYSTÈME DE VENTE
    // ======================
    public void Sell(PlayerData player, Product product)
    {
        if (!player.inventory.Contains(product))
        {
            Debug.Log("Produit introuvable");
            return;
        }

        player.inventory.Remove(product);
        player.money += product.sellPrice;

        int profit = product.sellPrice - product.buyPrice;
        player.profits.Add(profit);

        Debug.Log($"Vendu : {product.name}. Profit : {profit}");

        RefreshVisuals(); // Mise à jour visuelle
    }

    // ======================
    // 📊 CALCULS ET UTILITAIRES
    // ======================
    public int GetProfit(PlayerData player)
    {
        int total = 0;
        foreach (int p in player.profits) total += p;
        return total;
    }

    // Fonction interne pour rafraîchir l'affichage via le FoodVisualizer
    private void RefreshVisuals()
    {
        if (foodVisualizer != null)
            foodVisualizer.Refresh();

        if (moneyTextP1 != null)
            moneyTextP1.text = $"{player1.money}$";

        if (moneyTextP2 != null)
            moneyTextP2.text = $"{player2.money}$";
    }
}