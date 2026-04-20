using UnityEngine;

public class PlayerEconomy
{
    public int money = 100;

    public int totalEarned = 0;
    public int totalSpent = 0;

    public bool Buy(int cost)
    {
        if (money >= cost)
        {
            money -= cost;
            totalSpent += cost;
            return true;
        }

        return false;
    }

    public void Sell(int price)
    {
        if (price < 0) return;

        money += price;
        totalEarned += price;
    }

    public int GetProfit()
    {
        return totalEarned - totalSpent;
    }
}
