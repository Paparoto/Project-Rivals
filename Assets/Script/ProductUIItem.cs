using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ProductUIItem : MonoBehaviour
{
    [Header("Affichage Permanent")]
    public TMP_Text nameText;
    public TMP_Text buyPriceText;
    public TMP_Text stockText;

    [Header("Affichage Basculant (Détails)")]
    public GameObject detailsPanel; // Glisse l'objet DetailsPanel ici
    public TMP_Text sellPriceText;
    public TMP_Text bonusText;
    public TMP_Text malusText;

    [Header("Visuels")]
    public GameObject selectionFrame;

    private TransactionManager.Product data;
    private TransactionManager.PlayerData owner;

    public void Setup(TransactionManager.Product p, TransactionManager.PlayerData plr)
    {
        data = p; owner = plr;

        // On ne remplit que ce qui est visible au début
        nameText.text = p.name.ToUpper();
        buyPriceText.text = $"PRIX: {p.buyPrice}$";

        // On prépare les détails (cachés au début)
        sellPriceText.text = $"REVENTE: {p.sellPrice}$";
        bonusText.text = $"BONUS: {p.bonus}";
        malusText.text = $"MALUS: {p.malus}";

        UpdateStock();
        SetSelected(false);
    }

    public void UpdateStock()
    {
        int count = owner.inventory.FindAll(x => x.name == data.name).Count;
        stockText.text = $"STOCK: {count}";
    }

    public void SetSelected(bool isSelected)
    {
        // On affiche le cadre de sélection
        if (selectionFrame != null) selectionFrame.SetActive(isSelected);

        // LA CASE BASCULANTE : On affiche les détails seulement si sélectionné
        if (detailsPanel != null) detailsPanel.SetActive(isSelected);
    }
}