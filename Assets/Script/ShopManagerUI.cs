using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManagerUI : MonoBehaviour
{

    public PlayerMovement3D[] playerControllers; 
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

    [Header("Mod�les 3D des produits")]
    public GameObject prefabEntrecote;
    public GameObject prefabRoti;
    public GameObject prefabTomate;
    public GameObject prefabPoivron;
    public GameObject prefabGateau;
    public GameObject prefabCookie;
    public GameObject prefabThon;
    public GameObject prefabSaumon;
    public GameObject prefabFromageBleu;
    public GameObject prefabEmmental;
    public GameObject prefabPomme;
    public GameObject prefabMelon;

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
        // Ajoute bien le nom de la variable prefab � la fin de chaque ligne
        catalog.Add(new TransactionManager.Product("Tomate", "Legumes", 5, 5, "Lancer de tomate", "Invendable", prefabTomate));
        catalog.Add(new TransactionManager.Product("Entrecote", "Viande", 20, 40, "Clients +10%", "Plus lent (-5%)", prefabEntrecote));
        catalog.Add(new TransactionManager.Product("Boeuf", "Viande", 30, 70, "None", "Plus lent (-8%)", prefabRoti));
        catalog.Add(new TransactionManager.Product("Poivron", "Legumes", 5, 20, "Patience +10%", "None", prefabPoivron));
        catalog.Add(new TransactionManager.Product("Gateau", "Divers", 50, 75, "Clients +10%", "Patience -5%", prefabGateau));
        catalog.Add(new TransactionManager.Product("Cookies", "Divers", 5, 15, "Plus rapide +20%", "Patience -10%", prefabCookie));
        catalog.Add(new TransactionManager.Product("Thon", "Poisson", 40, 40, "Revenus +10%", "Clients -10%", prefabThon));
        catalog.Add(new TransactionManager.Product("Saumon", "Poisson", 10, 15, "Revenus +5%", "None", prefabSaumon));
        catalog.Add(new TransactionManager.Product("Fromage bleu", "Divers", 30, 80, "None", "Clients -10%", prefabFromageBleu));
        catalog.Add(new TransactionManager.Product("Emmental", "Divers", 10, 55, "Revenus -5%", "None", prefabEmmental));
        catalog.Add(new TransactionManager.Product("pomme", "Legumes", 10, 15, "Vitesse +10%", "None", prefabPomme));
        catalog.Add(new TransactionManager.Product("Melon", "Legumes", 20, 30, "Vitesse +8%", "None", prefabMelon));

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

        // On passe le produit et le joueur (le manager est d�j� accessible via transactionManager)
        ui.Setup(p, owner);

        return ui;
    }

    // Le reste du code (Update, Navigate, TryBuy) NE CHANGE PAS.

    void Update()
{
        for (int i = 0; i < p1Rows.Count; i++)
        p1Rows[i].UpdateStock();
    for (int i = 0; i < p2Rows.Count; i++)
        p2Rows[i].UpdateStock();
    
    if (p1Rows.Count == 0 || p2Rows.Count == 0) return;
    // --- JOUEUR 1 (W / S / E) + Manette 1 (joystick haut/bas + carré = button 2) ---
    if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown("joystick 1 button 6")) 
        Navigate(ref p1Index, -1, p1Rows, p1Scroll);
    if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown("joystick 1 button 7")) 
        Navigate(ref p1Index, 1, p1Rows, p1Scroll);
    if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown("joystick 1 button 0"))
{
    int p1Stock = transactionManager.player1.inventory.FindAll(x => x.name == catalog[p1Index].name).Count;
    if (playerControllers[0].isInPCZone && p1Stock < 3)
    {
        if (transactionManager.Buy(transactionManager.player1, catalog[p1Index]))
            p1Rows[p1Index].UpdateStock();
    }
}


    // --- JOUEUR 2 (Flèches / RightShift) + Manette 2 (joystick haut/bas + carré = button 2) ---
    if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown("joystick 2 button 6")) 
        Navigate(ref p2Index, -1, p2Rows, p2Scroll);
    if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown("joystick 2 button 7")) 
        Navigate(ref p2Index, 1, p2Rows, p2Scroll);
    if (Input.GetKeyDown(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown("joystick 2 button 0"))
{
    int p2Stock = transactionManager.player2.inventory.FindAll(x => x.name == catalog[p2Index].name).Count;
    if (playerControllers[1].isInPCZone && p2Stock < 3)
    {
        if (transactionManager.Buy(transactionManager.player2, catalog[p2Index]))
            p2Rows[p2Index].UpdateStock();
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