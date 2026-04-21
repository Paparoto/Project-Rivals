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
    public bool isAtCounter = false;

    [Header("Requête en cours")]
    public string requestedProductName;
    public string requestedProductCategory;
    public int requestedProductPrice;

    private const float arrivalThreshold = 0.1f;
    [HideInInspector] public QueueManager queueManager;

    private CustomerSkinManager skinManager;

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
            TransactionManager.PlayerData player = assignedPlayer == 1
                ? transactionManager.player1
                : transactionManager.player2;

            GameObject result = Request(player);

            if (result == null)
            {
                queueManager.RemoveClient(gameObject);
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
            return null;

        int randomIndex = Random.Range(0, player.inventory.Count);
        requestedProduct = player.inventory[randomIndex];

        requestedProductName     = requestedProduct.name;
        requestedProductCategory = requestedProduct.category;
        requestedProductPrice    = requestedProduct.sellPrice;

        return this.gameObject;
    }

    public void Leave(Vector3 exitPos)
    {
        if (isLeaving) return;
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
}