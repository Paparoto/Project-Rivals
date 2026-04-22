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
    public Image fondBulle;
    private bool hasChosenProduct = false;

    [Header("Patience")]
    public float maxPatience = 10f; // Temps d'attente max
    private float currentPatience;
    private bool isWaiting = false; // Est-ce qu'on est en train de compter ?

    public void SetTarget(Vector3 newPos)
    {
        if (skinManager == null)
            skinManager = GetComponent<CustomerSkinManager>();

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
        if (isWaiting && !isLeaving)
        {
            currentPatience -= Time.deltaTime;

            // --- CHANGEMENT DE COULEUR ---
            if (fondBulle != null)
            {
                // Calcul du ratio (1 au début, 0 à la fin)
                float ratio = currentPatience / maxPatience;

                // Transition du rouge (fin) vers le vert (début)
                // Quand ratio = 1 (début), c'est Vert. Quand ratio = 0 (fin), c'est Rouge.
                fondBulle.color = Color.Lerp(Color.red, Color.green, ratio);
            }

            // On s'assure que le texte reste bien noir
            if (texteProduit != null)
            {
                texteProduit.color = Color.black;
            }

            // --- FIN DE PATIENCE ---
            if (currentPatience <= 0)
            {
                isWaiting = false;
                if (queueManager != null) queueManager.RemoveClient(this.gameObject);
            }
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
            TransactionManager.PlayerData player = (assignedPlayer == 1)
                ? transactionManager.player1
                : transactionManager.player2;

            GameObject result = Request(player);

            if (result == null)
            {
                Debug.Log("Client : Magasin vide, je m'en vais !");
                if (queueManager != null)
                {
                    queueManager.RemoveClient(this.gameObject);
                }
            }
            else
            {
                Debug.Log("Client : Commande prête, j'attends le joueur.");
            }
        }
        if (!isAtCounter) return; // S'il n'est pas encore au comptoir, il ne s'impatiente pas encore

        if (transactionManager != null)
        {
            // ... (ton code existant : définition du player, Request, affichage bulle) ...

            // --- NOUVEAU : On lance le chrono de patience ---
            currentPatience = maxPatience;
            isWaiting = true;
        }
        if (bulleObjet != null && fondBulle != null)
        {
            fondBulle.color = Color.green; // La bulle commence bien verte
                                           // ... le reste de ton code ...
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

        // Filtrer les tomates
        var filteredInventory = player.inventory.FindAll(p => p.name != "Tomate");

        // S'il n'y a que des tomates (ou rien d'autre), le client s'en va
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

        isWaiting = false;

        if (bulleObjet != null) bulleObjet.SetActive(false);
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
        }
    }

    private void CacherBulle()
    {
        if (bulleObjet != null) bulleObjet.SetActive(false);
    }
}