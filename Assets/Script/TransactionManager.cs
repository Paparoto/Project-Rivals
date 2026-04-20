using System.Collections.Generic;
using UnityEngine;

public class TransactionManager : MonoBehaviour
{
    // ======================
    // 📦 PRODUCT (INCLUS ICI)
    // ======================
    public class Product
    {
        public string name;
        public int buyPrice;

        public Product(string name, int buyPrice)
        {
            this.name = name;
            this.buyPrice = buyPrice;
        }
    }

    // ======================
    // 👤 JOUEURS
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
    public bool Buy(PlayerData player, string productName, int buyPrice)
    {
        if (player.money < buyPrice)
        {
            Debug.Log("Pas assez d'argent");
            return false;
        }

        player.money -= buyPrice;
        player.inventory.Add(new Product(productName, buyPrice));

        return true;
    }

    // ======================
    // 💸 VENTE
    // ======================
    public void Sell(PlayerData player, Product product, int sellPrice)
    {
        if (!player.inventory.Contains(product))
        {
            Debug.Log("Produit introuvable");
            return;
        }

        player.inventory.Remove(product);
        player.money += sellPrice;

        int profit = sellPrice - product.buyPrice;
        player.profits.Add(profit);

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