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
        if (TransactionManager.Instance == null) return;

        // 1. Trouver le bon joueur et son inventaire
        List<TransactionManager.Product> inv;
        PlayerMovement3D targetPlayerScript = null;
        PlayerMovement3D[] allPlayers = FindObjectsByType<PlayerMovement3D>(FindObjectsSortMode.None);

        if (targetPlayer == PlayerChoice.Player1)
        {
            inv = TransactionManager.Instance.player1.inventory;
            targetPlayerScript = System.Array.Find(allPlayers, p => p.gamepadIndex == 0);
        }
        else
        {
            inv = TransactionManager.Instance.player2.inventory;
            targetPlayerScript = System.Array.Find(allPlayers, p => p.gamepadIndex == 1);
        }

        // 2. Nom de l'objet porté par ce joueur
        string itemInHand = (targetPlayerScript != null) ? targetPlayerScript.carriedProductName : "";

        Dictionary<string, int> displayedCount = new Dictionary<string, int>();

        foreach (Transform child in transform)
        {
            string cleanName = CleanName(child.name);

            // Nombre d'exemplaires dans l'inventaire
            int amountToDisplay = inv.Count(p => p.name == cleanName);

            // SI LE JOUEUR TIENT CET OBJET, ON EN CACHE UN SUR L'ÉTAGÈRE
            if (cleanName == itemInHand)
            {
                amountToDisplay--;
            }

            if (!displayedCount.ContainsKey(cleanName)) displayedCount[cleanName] = 0;

            if (displayedCount[cleanName] < amountToDisplay)
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