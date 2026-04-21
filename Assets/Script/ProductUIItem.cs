using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ProductUIItem : MonoBehaviour
{
    [Header("Affichage Permanent")]
    public TMP_Text nameText;
    public TMP_Text buyPriceText;
    public TMP_Text stockText;

    [Header("Affichage Basculant (D�tails)")]
    public GameObject detailsPanel; // Glisse l'objet DetailsPanel ici
    public TMP_Text sellPriceText;
    public TMP_Text bonusText;
    public TMP_Text malusText;

    [Header("Visuels")]
    public GameObject selectionFrame;

    private TransactionManager.Product data;
    private TransactionManager.PlayerData owner;
    [Header("Police")]
    public TMP_FontAsset font;

    public void Setup(TransactionManager.Product p, TransactionManager.PlayerData plr)
    {
        data = p; owner = plr;

        // On ne remplit que ce qui est visible au d�but
        if (font != null)
    {
        nameText.font = font;
        buyPriceText.font = font;
        stockText.font = font;
        sellPriceText.font = font;
        bonusText.font = font;
        malusText.font = font;
    }
        
        buyPriceText.text = $"{p.buyPrice}$";
        nameText.text = p.name.ToUpper();
        sellPriceText.text = $"REVENTE: {p.sellPrice}$";
        if (p.bonus == "None"){ bonusText.text ="";}
        else bonusText.text = $"BONUS: {p.bonus}";
        if (p.malus == "None"){ malusText.text ="";}
        else malusText.text = $"MALUS: {p.malus}";

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
        // On affiche le cadre de s�lection
        if (selectionFrame != null) selectionFrame.SetActive(isSelected);

        // LA CASE BASCULANTE : On affiche les d�tails seulement si s�lectionn�
        if (detailsPanel != null) detailsPanel.SetActive(isSelected);
    }
}