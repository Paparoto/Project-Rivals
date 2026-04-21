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
        Invoke("SpawnClient", spawnDelay + 0.5f);
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

                if (playerTakeScript != null)
                {
                    TransactionManager.PlayerData pData = (assignedPlayer == 1) ? transactionManager.player1 : transactionManager.player2;

                    if (pData.inventory.Count > 0)
                    {
                        client.Request(pData);
                        if (client.requestedProduct != null)
                        {
                            playerTakeScript.objectToPickup = client.requestedProduct.prefab;
                            Debug.Log("Produit envoyé au joueur : " + client.requestedProduct.name);
                        }
                        else
                        {
                            Debug.LogError("Le produit demandé est null !");
                        }
                    }
                    else
                    {
                        Debug.LogWarning("L'inventaire du Joueur " + assignedPlayer + " est VIDE. Le client ne peut rien demander.");
                    }
                }
                else
                {
                    Debug.LogError("Le slot PlayerTakeScript est VIDE dans l'inspecteur du QueueManager !");
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