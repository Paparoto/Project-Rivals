using System.Collections.Generic;
using UnityEngine;

public class TransactionManager : MonoBehaviour
{
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

        public Product(string name, string category, int buyPrice, int sellPrice, string bonus, string malus)
        {
            this.name = name;
            this.category = category;
            this.buyPrice = buyPrice;
            this.sellPrice = sellPrice;
            this.bonus = bonus;
            this.malus = malus;
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