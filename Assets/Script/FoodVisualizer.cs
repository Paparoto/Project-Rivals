using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class FoodVisualizer : MonoBehaviour
{
    // Choix du joueur à afficher pour CET objet "Food"
    public enum PlayerChoice { Player1, Player2 }
    [Header("Quel joueur afficher ?")]
    public PlayerChoice targetPlayer;

    public void Refresh()
    {
        // On utilise l'Instance statique pour être sûr d'avoir le bon Manager
        if (TransactionManager.Instance == null) return;

        // On prend la liste du bon joueur
        List<TransactionManager.Product> inv;
        if (targetPlayer == PlayerChoice.Player1)
            inv = TransactionManager.Instance.player1.inventory;
        else
            inv = TransactionManager.Instance.player2.inventory;

        Dictionary<string, int> displayedCount = new Dictionary<string, int>();

        foreach (Transform child in transform)
        {
            string cleanName = CleanName(child.name);
            int amountInInventory = inv.Count(p => p.name == cleanName);

            if (!displayedCount.ContainsKey(cleanName)) displayedCount[cleanName] = 0;

            if (displayedCount[cleanName] < amountInInventory)
            {
                child.gameObject.SetActive(true);
                displayedCount[cleanName]++;
            }
            else
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    private string CleanName(string name)
    {
        // Au lieu de couper au premier espace, on enlève " (" 
        // qui arrive avant le chiffre (1), (2), etc.
        int index = name.IndexOf(" (");
        if (index != -1)
        {
            return name.Substring(0, index);
        }
        return name;
    }
}