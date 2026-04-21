using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManagerUI : MonoBehaviour
{
    public TransactionManager transactionManager;
    public GameObject productPrefab;

    [Header("Player 1")]
    public Transform p1Content;
    public ScrollRect p1Scroll;
    private List<ProductUIItem> p1Rows = new List<ProductUIItem>();
    private int p1Index = 0;

    [Header("Player 2")]
    public Transform p2Content;
    public ScrollRect p2Scroll;
    private List<ProductUIItem> p2Rows = new List<ProductUIItem>();
    private int p2Index = 0;

    [Header("Style")]
    public Color selectedColor = new Color(0f, 1f, 0f, 0.4f); // Vert fluo terminal
    public Color normalColor = new Color(0f, 0f, 0f, 0.6f);   // Noir transparent

    void Start()
    {
        CreateCatalog();
        // Sélectionne le premier item par défaut
        UpdateVisuals();
    }

    void CreateCatalog()
    {
        List<TransactionManager.Product> catalog = new List<TransactionManager.Product>();

        // --- AJOUT DES PRODUITS (EXCEL) ---
        catalog.Add(new TransactionManager.Product("Entrecote", "Viande", 30, 75, "None", "Plus lent (-20%)"));
        catalog.Add(new TransactionManager.Product("Poulet roti", "Viande", 15, 40, "None", "Refroidir (Prix:30)"));
        catalog.Add(new TransactionManager.Product("Tomate", "Legume", 5, 15, "Lancer tomate", "Pourri vite"));
        catalog.Add(new TransactionManager.Product("Oignons", "Legume", 5, 20, "Ne pourri pas", "Reduit client-10%"));
        catalog.Add(new TransactionManager.Product("Gateau", "Dessert", 30, 75, "None", "Stock +2"));
        catalog.Add(new TransactionManager.Product("Cookie", "Dessert", 5, 15, "Plus rapide +20%", "None"));
        catalog.Add(new TransactionManager.Product("Thon", "Poisson", 15, 45, "None", "Reduit client-10%"));
        catalog.Add(new TransactionManager.Product("Sardine", "Poisson", 10, 30, "None", "Chat qui vole"));
        catalog.Add(new TransactionManager.Product("Fromage bleu", "Fromage", 30, 80, "None", "Odeur forte"));
        catalog.Add(new TransactionManager.Product("Emmental", "Fromage", 10, 25, "None", "None"));
        catalog.Add(new TransactionManager.Product("Banane", "Fruit", 5, 15, "Lancer banane", "None"));
        catalog.Add(new TransactionManager.Product("Ananase", "Fruit", 20, 50, "None", "Pourri (moyen)"));

        foreach (var p in catalog)
        {
            // Spawn P1
            GameObject item1 = Instantiate(productPrefab, p1Content);
            ProductUIItem ui1 = item1.GetComponent<ProductUIItem>();
            ui1.Setup(p, transactionManager.player1, transactionManager);
            p1Rows.Add(ui1);

            // Spawn P2
            GameObject item2 = Instantiate(productPrefab, p2Content);
            ProductUIItem ui2 = item2.GetComponent<ProductUIItem>();
            ui2.Setup(p, transactionManager.player2, transactionManager);
            p2Rows.Add(ui2);
        }
    }

    void Update()
    {
        // Sécurité : on ne fait rien si les listes ne sont pas prêtes
        if (p1Rows.Count == 0 || p2Rows.Count == 0) return;

        // --- JOUEUR 1 (W / S pour naviguer, E pour acheter) ---
        if (Input.GetKeyDown(KeyCode.W)) Navigate(ref p1Index, -1, p1Rows, p1Scroll);
        if (Input.GetKeyDown(KeyCode.S)) Navigate(ref p1Index, 1, p1Rows, p1Scroll);
        if (Input.GetKeyDown(KeyCode.E))
        {
            p1Rows[p1Index].buyButton.onClick.Invoke();
        }

        // --- JOUEUR 2 (Flèches pour naviguer, Shift pour acheter) ---
        if (Input.GetKeyDown(KeyCode.UpArrow)) Navigate(ref p2Index, -1, p2Rows, p2Scroll);
        if (Input.GetKeyDown(KeyCode.DownArrow)) Navigate(ref p2Index, 1, p2Rows, p2Scroll);

        // On accepte le Shift Droit (plus près des flèches) ou le Shift Gauche
        if (Input.GetKeyDown(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.LeftShift))
        {
            p2Rows[p2Index].buyButton.onClick.Invoke();
        }
    }

    void Navigate(ref int index, int dir, List<ProductUIItem> list, ScrollRect scroll)
    {
        index = Mathf.Clamp(index + dir, 0, list.Count - 1);

        // Calcul pour que le scroll suive la sélection
        float targetPos = 1f - ((float)index / (list.Count - 1));
        scroll.verticalNormalizedPosition = targetPos;

        UpdateVisuals();
    }

    void UpdateVisuals()
    {
        for (int i = 0; i < p1Rows.Count; i++)
            p1Rows[i].GetComponent<Image>().color = (i == p1Index) ? selectedColor : normalColor;

        for (int i = 0; i < p2Rows.Count; i++)
            p2Rows[i].GetComponent<Image>().color = (i == p2Index) ? selectedColor : normalColor;
    }
}