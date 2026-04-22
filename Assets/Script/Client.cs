using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Client : MonoBehaviour
{
    public float speed = 5f;
    private Vector3 targetPosition;
    private bool isLeaving = false;
    private bool hasRequestedAtCounter = false;

    public TransactionManager transactionManager;
    public TransactionManager.Product requestedProduct;
    public Transform exitPoint;

    [Header("Joueur assigné")]
    public int assignedPlayer = 1;
    public bool isAtCounter = false;

    [Header("Requête en cours")]
    public string requestedProductName;
    public string requestedProductCategory;
    public int requestedProductPrice;

    private const float arrivalThreshold = 0.1f;
    [HideInInspector] public QueueManager queueManager;

    private CustomerSkinManager skinManager;

    [Header("Bulle de Dialogue")]
    public GameObject bulleObjet;
    public TMP_Text texteProduit;

    private bool hasChosenProduct = false;

    public void SetTarget(Vector3 newPos)
    {
        if (skinManager == null)
            skinManager = GetComponent<CustomerSkinManager>();

        // Ne lancer la course que si la cible est suffisamment loin
        bool shouldMove = Vector3.Distance(transform.position, newPos) > arrivalThreshold;

        targetPosition = newPos;

        if (shouldMove)
        {
            Vector3 direction = newPos - transform.position;
            direction.y = 0;
            if (direction != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(direction);

            if (skinManager != null) skinManager.SetRunning(true);
        }

        if (isAtCounter) return;

        isAtCounter = false;
        hasRequestedAtCounter = false;
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < arrivalThreshold)
        {
            if (isLeaving)
                Destroy(gameObject);
            else if (!hasRequestedAtCounter)
                OnReachedTarget();
        }
    }

    private void OnReachedTarget()
    {
        if (skinManager == null)
            skinManager = GetComponent<CustomerSkinManager>();

        if (skinManager != null) skinManager.SetRunning(false);

        transform.rotation = Quaternion.Euler(0, 180f, 0);

        hasRequestedAtCounter = true;
        if (!isAtCounter) return;

        if (transactionManager != null)
        {
            // On déclare 'player' UNE SEULE FOIS ici
            TransactionManager.PlayerData player = (assignedPlayer == 1)
                ? transactionManager.player1
                : transactionManager.player2;

            // On fait la requête
            GameObject result = Request(player);

            if (result == null)
            {
                // S'il n'y a rien en stock, le client s'en va
                Debug.Log("Client : Magasin vide, je m'en vais !");
                if (queueManager != null)
                {
                    queueManager.RemoveClient(this.gameObject);
                }
            }
            else
            {
                // S'il y a un produit, il attend qu'on lui parle (Bouton 1)
                // On ne fait rien, la bulle s'affichera lors de l'interaction
                Debug.Log("Client : Commande prête, j'attends le joueur.");
            }
        }
    }

    public void SetAsFirstInQueue()
    {
        isAtCounter = true;

        if (hasRequestedAtCounter) return;

        if (Vector3.Distance(transform.position, targetPosition) < arrivalThreshold)
            OnReachedTarget();
    }

    public GameObject Request(TransactionManager.PlayerData player)
    {
        if (hasChosenProduct) return this.gameObject;

        if (player.inventory == null || player.inventory.Count == 0)
            return null;

        // Filtrer les tomates de l'inventaire
        var filteredInventory = player.inventory.FindAll(p => p.name != "Tomate");

        if (filteredInventory.Count == 0)
            return null;

        int randomIndex = Random.Range(0, filteredInventory.Count);
        requestedProduct = filteredInventory[randomIndex];

        requestedProductName = requestedProduct.name;
        requestedProductCategory = requestedProduct.category;
        requestedProductPrice = requestedProduct.sellPrice;

        hasChosenProduct = true;
        return this.gameObject;
    }

    public void Leave(Vector3 exitPos)
    {
        if (isLeaving) return;

        if (bulleObjet != null) bulleObjet.SetActive(false); // Cache la bulle
        if (skinManager == null)
            skinManager = GetComponent<CustomerSkinManager>();

        isLeaving = true;
        targetPosition = exitPos;

        Vector3 direction = exitPos - transform.position;
        direction.y = 0;
        if (direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direction);

        if (skinManager != null) skinManager.SetRunning(true);

        requestedProductName     = "";
        requestedProductCategory = "";
        requestedProductPrice    = 0;
    }
    public void AfficherMaBulle()
    {
        if (bulleObjet != null && texteProduit != null)
        {
            texteProduit.text = "Je veux : " + requestedProductName;
            bulleObjet.SetActive(true);

            // On a supprimé le Invoke("CacherBulle") pour que ça reste affiché !
        }
    }

    private void CacherBulle()
    {
        if (bulleObjet != null) bulleObjet.SetActive(false);
    }
}