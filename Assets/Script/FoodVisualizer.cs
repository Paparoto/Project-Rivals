using UnityEngine;
using System.Collections.Generic;
using System.Linq; // Important pour faciliter le comptage

public class FoodVisualizer : MonoBehaviour
{
    public TransactionManager transactionManager;

    public void Refresh()
    {
        if (transactionManager == null || transactionManager.player1 == null) return;

        // 1. On récupère l'inventaire
        List<TransactionManager.Product> inv = transactionManager.player1.inventory;

        // 2. On crée un dictionnaire pour compter combien on a affiché de chaque objet
        // Clé : Nom de l'objet (ex: "Pomme"), Valeur : Nombre déjà activés
        Dictionary<string, int> displayedCount = new Dictionary<string, int>();

        // 3. On boucle sur tous les enfants (tes modèles 3D)
        foreach (Transform child in transform)
        {
            // On nettoie le nom (Unity ajoute souvent " (1)" ou " (Clone)", on l'enlève)
            string cleanName = CleanName(child.name);

            // On compte combien on a de ce produit dans l'inventaire
            int amountInInventory = inv.Count(p => p.name == cleanName);

            // On regarde combien on en a déjà affiché
            if (!displayedCount.ContainsKey(cleanName)) {
                displayedCount[cleanName] = 0;
            }

            // Si on en a affiché moins que ce qu'on possède, on l'active
            if (displayedCount[cleanName] < amountInInventory)
            {
                child.gameObject.SetActive(true);
                displayedCount[cleanName]++; // On incrémente le compteur
            }
            else
            {
                child.gameObject.SetActive(false); // On cache le surplus
            }
        }
    }

    // Petite fonction pour ignorer les " (1)" ou " (Clone)" dans les noms Unity
    private string CleanName(string name)
    {
        if (name.Contains(" "))
            return name.Split(' ')[0];
        return name;
    }
}