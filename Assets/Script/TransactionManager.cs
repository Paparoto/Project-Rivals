using System.Collections.Generic;
using UnityEngine;

public class TransactionManager : MonoBehaviour
{
    [Header("Modèles des Produits")]
    public GameObject appleModel;
    // ... (ajoute les autres modèles)

    void Start()
    {
        // Produits de test
        Product apple = new Product("Pomme", "Fruit", 2, 5, "HP", "Aucun", appleModel);

        player1.inventory.AddRange(new List<Product> {apple});
        player1.money = 500;

        Debug.Log($"Inventaire player1 initialisé : {player1.inventory.Count} produits, {player1.money} pièces.");
    }
    // ======================
    // 📦 PRODUCT
    // ======================
    public class Product
    {
        public string name;
        public string category;
        public int buyPrice;
        public int sellPrice;
        public string bonus;
        public string malus;

        // --- AJOUTE CETTE LIGNE ---
        public GameObject prefab;

        // MODIFIE LE CONSTRUCTEUR POUR INCLURE LE PREFAB
        public Product(string name, string category, int buyPrice, int sellPrice, string bonus, string malus, GameObject prefab)
        {
            this.name = name;
            this.category = category;
            this.buyPrice = buyPrice;
            this.sellPrice = sellPrice;
            this.bonus = bonus;
            this.malus = malus;
            this.prefab = prefab; // <-- Assigne le prefab ici
        }
    }

    // ======================
    // 👤 PLAYER
    // ======================
    public class PlayerData
    {
        public int money = 100;
        public List<Product> inventory = new List<Product>();
        public List<int> profits = new List<int>();
    }

    public PlayerData player1 = new PlayerData();
    public PlayerData player2 = new PlayerData();

    // ======================
    // 🛒 ACHAT
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

        Debug.Log("Achat: " + product.name + " (" + product.category + ")");
        Debug.Log("Bonus: " + product.bonus);
        Debug.Log("Malus: " + product.malus);

        return true;
    }

    // ======================
    // 💸 VENTE
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

        Debug.Log("Vente: " + product.name);
        Debug.Log("Profit: " + profit);
    }

    // ======================
    // 📊 PROFIT TOTAL
    // ======================
    public int GetProfit(PlayerData player)
    {
        int total = 0;

        foreach (int p in player.profits)
            total += p;

        return total;
    }
}