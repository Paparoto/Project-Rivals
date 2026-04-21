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
    public Color selectedColor = new Color(0f, 1f, 0f, 0.4f);
    public Color normalColor = new Color(0f, 0f, 0f, 0.6f);

    // LISTE DES PRODUITS ACCESSIBLE PARTOUT
    private List<TransactionManager.Product> catalog = new List<TransactionManager.Product>();

    void Start()
    {
        CreateCatalog();
        UpdateVisuals();
    }

    void CreateCatalog()
    {
        catalog.Clear();
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
            p1Rows.Add(CreateRow(p, p1Content, transactionManager.player1));
            p2Rows.Add(CreateRow(p, p2Content, transactionManager.player2));
        }
    }

    // Dans ShopManagerUI.cs, modifie juste la fonction CreateRow :

    ProductUIItem CreateRow(TransactionManager.Product p, Transform parent, TransactionManager.PlayerData owner)
    {
        GameObject go = Instantiate(productPrefab, parent);
        ProductUIItem ui = go.GetComponent<ProductUIItem>();

        // On passe le produit et le joueur (le manager est déjà accessible via transactionManager)
        ui.Setup(p, owner);

        return ui;
    }

    // Le reste du code (Update, Navigate, TryBuy) NE CHANGE PAS.

    void Update()
    {
        if (p1Rows.Count == 0 || p2Rows.Count == 0) return;

        // --- JOUEUR 1 (W / S / E) ---
        if (Input.GetKeyDown(KeyCode.W)) Navigate(ref p1Index, -1, p1Rows, p1Scroll);
        if (Input.GetKeyDown(KeyCode.S)) Navigate(ref p1Index, 1, p1Rows, p1Scroll);
        if (Input.GetKeyDown(KeyCode.E))
        {
            // EXECUTE L'ACHAT POUR LE P1
            if (transactionManager.Buy(transactionManager.player1, catalog[p1Index]))
            {
                p1Rows[p1Index].UpdateStock(); // Met à jour le texte Stk:X
            }
        }

        // --- JOUEUR 2 (Flèches / RightShift) ---
        if (Input.GetKeyDown(KeyCode.UpArrow)) Navigate(ref p2Index, -1, p2Rows, p2Scroll);
        if (Input.GetKeyDown(KeyCode.DownArrow)) Navigate(ref p2Index, 1, p2Rows, p2Scroll);

        if (Input.GetKeyDown(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.LeftShift))
        {
            // EXECUTE L'ACHAT POUR LE P2
            if (transactionManager.Buy(transactionManager.player2, catalog[p2Index]))
            {
                p2Rows[p2Index].UpdateStock(); // Met à jour le texte Stk:X
            }
        }
    }

    void Navigate(ref int index, int dir, List<ProductUIItem> list, ScrollRect scroll)
    {
        index = Mathf.Clamp(index + dir, 0, catalog.Count - 1);

        float targetPos = 1f - ((float)index / (catalog.Count - 1));
        scroll.verticalNormalizedPosition = targetPos;

        UpdateVisuals();
    }

    void UpdateVisuals()
    {
        for (int i = 0; i < p1Rows.Count; i++)
        {
            bool sel = (i == p1Index);
            p1Rows[i].SetSelected(sel);
            p1Rows[i].GetComponent<Image>().color = sel ? selectedColor : normalColor;
        }

        for (int i = 0; i < p2Rows.Count; i++)
        {
            bool sel = (i == p2Index);
            p2Rows[i].SetSelected(sel);
            p2Rows[i].GetComponent<Image>().color = sel ? selectedColor : normalColor;
        }
    }
}