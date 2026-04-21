using UnityEngine;
using System.Collections.Generic;

public class QueueManager : MonoBehaviour
{
    [Header("Références")]
    public GameObject clientPrefab;
    public Transform[] waitPoints;
    public Transform spawnPoint;
    public Transform exitPoint;
    public TransactionManager transactionManager;

    [Header("Joueur associé à cette file")]
    public int assignedPlayer = 1;

    [Header("Réglages Spawn")]
    public float spawnDelay = 2f;
    public float minInterval = 7f;
    public float maxInterval = 10f;

    [Header("Lien avec le ramassage")]
    public TakeObject playerTakeScript; // Glisse ici le Joueur (celui qui a le script TakeObject)

    private List<GameObject> clientList = new List<GameObject>();

    void Start()
    {
        Invoke("SpawnClient", spawnDelay);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
            ServeClient();
    }

    void SpawnClient()
    {
        if (clientList.Count < waitPoints.Length)
        {
            GameObject newClient = Instantiate(clientPrefab, spawnPoint.position, Quaternion.identity);

            CustomerSkinManager skinManager = newClient.GetComponent<CustomerSkinManager>();
            if (skinManager != null) skinManager.ApplyRandomSkin();

            Client client = newClient.GetComponent<Client>();
            client.transactionManager = transactionManager;
            client.exitPoint = exitPoint;
            client.assignedPlayer = assignedPlayer;
            client.queueManager = this;
            clientList.Add(newClient);
            UpdateQueuePositions();
        }

        Invoke("SpawnClient", Random.Range(minInterval, maxInterval));
    }

    public void ServeClient()
    {
        if (clientList.Count > 0)
        {
            // ... (ton code existant)

            if (playerTakeScript != null)
                playerTakeScript.objectToPickup = null; // On vide la main du joueur

            UpdateQueuePositions();
        }
    }

    void UpdateQueuePositions()
    {
        for (int i = clientList.Count - 1; i >= 0; i--)
        {
            if (clientList[i] == null)
            {
                clientList.RemoveAt(i);
                continue;
            }

            Client client = clientList[i].GetComponent<Client>();
            client.SetTarget(waitPoints[i].position);

            if (i == 0)
            {
                client.SetAsFirstInQueue();

                // --- AJOUT POUR LE PRODUIT ---
                if (playerTakeScript != null)
                {
                    // On récupère les données du joueur (1 ou 2)
                    TransactionManager.PlayerData playerData = (assignedPlayer == 1)
                        ? transactionManager.player1
                        : transactionManager.player2;

                    // On force le client à faire sa demande
                    client.Request(playerData);

                    // On envoie le prefab du produit au script TakeObject du joueur
                    // NOTE: Cela suppose que ta classe 'Product' a une variable 'prefab'
                    if (client.requestedProduct != null)
                    {
                        playerTakeScript.objectToPickup = client.requestedProduct.prefab;
                    }
                }
            }
        }
    }

    public void RemoveClient(GameObject client)
    {
        if (clientList.Contains(client))
        {
            clientList.Remove(client);
            client.GetComponent<Client>().Leave(exitPoint.position);
            UpdateQueuePositions();
        }
    }
}