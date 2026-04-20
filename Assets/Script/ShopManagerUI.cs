using System.Collections.Generic;
using UnityEngine;

public class ShopManagerUI : MonoBehaviour
{
    public TransactionManager transactionManager;
    public GameObject productPrefab;

    public Transform p1Content;
    public Transform p2Content;

    void Start()
    {
        // Define your catalog of products
        List<TransactionManager.Product> catalog = new List<TransactionManager.Product>();
        catalog.Add(new TransactionManager.Product("Apple", "Food", 10, 15, "Health+", "None"));
        catalog.Add(new TransactionManager.Product("Sword", "Weapon", 50, 40, "Attack+", "Weight+"));

        // Spawn for Player 1
        foreach (var p in catalog)
        {
            GameObject item = Instantiate(productPrefab, p1Content);
            item.GetComponent<ProductUIItem>().Setup(p, transactionManager.player1, transactionManager);
        }

        // Spawn for Player 2
        foreach (var p in catalog)
        {
            GameObject item = Instantiate(productPrefab, p2Content);
            item.GetComponent<ProductUIItem>().Setup(p, transactionManager.player2, transactionManager);
        }
    }
}