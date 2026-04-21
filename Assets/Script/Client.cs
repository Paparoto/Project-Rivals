using UnityEngine;

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
    public bool isAtCounter = false; // ← visible dans l'Inspector

    [Header("Requête en cours")]
    public string requestedProductName;
    public string requestedProductCategory;
    public int requestedProductPrice;

    private const float arrivalThreshold = 0.1f;
    [HideInInspector] public QueueManager queueManager; // ← injecté par QueueManager

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
        hasRequestedAtCounter = true;

        if (!isAtCounter) return;

        if (transactionManager != null)
        {
            TransactionManager.PlayerData player = assignedPlayer == 1
                ? transactionManager.player1
                : transactionManager.player2;

            GameObject result = Request(player);

            if (result == null)
            {
                Debug.Log("Inventaire vide, le client repart.");
                queueManager.RemoveClient(gameObject); // ← retire de la liste PUIS part
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
        if (player.inventory == null || player.inventory.Count == 0)
        {
            Debug.Log("L'inventaire du joueur est vide.");
            return null;
        }

        int randomIndex = Random.Range(0, player.inventory.Count);
        requestedProduct = player.inventory[randomIndex];

        requestedProductName     = requestedProduct.name;
        requestedProductCategory = requestedProduct.category;
        requestedProductPrice    = requestedProduct.sellPrice;

        Debug.Log($"[Joueur {assignedPlayer}] Le client demande : {requestedProduct.name} ({requestedProduct.category}) — {requestedProduct.sellPrice} pièces");
        return this.gameObject;
    }

    public void SetTarget(Vector3 newPos)
    {
        targetPosition = newPos;

        // Si le client est déjà au comptoir, on ne touche pas à sa requête
        if (isAtCounter) return;

        isAtCounter = false;
        hasRequestedAtCounter = false;
    }

    public void Leave(Vector3 exitPos)
    {
        if (isLeaving) return; // ← évite d'appeler Leave() deux fois sur le même client

        isLeaving = true;
        targetPosition = exitPos;

        requestedProductName     = "";
        requestedProductCategory = "";
        requestedProductPrice    = 0;
    }
}