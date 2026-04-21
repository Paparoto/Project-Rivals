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
            GameObject clientToLeave = clientList[0];
            clientList.RemoveAt(0);

            if (clientToLeave != null)
                clientToLeave.GetComponent<Client>().Leave(exitPoint.position);

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

                // On demande au client de choisir un produit dans l'inventaire
                // Pour que le joueur puisse ensuite lire cette info
                TransactionManager.PlayerData pData = (assignedPlayer == 1) ? transactionManager.player1 : transactionManager.player2;

                if (pData.inventory.Count > 0)
                {
                    client.Request(pData);
                    // Note : On ne "pousse" plus l'info vers le joueur, 
                    // c'est le joueur qui viendra lire 'GetFirstClientRequest'
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

    // Cette fonction est utilisée par le TakeObject du joueur pour savoir quoi ramasser
    public GameObject GetFirstClientRequest()
    {
        if (clientList.Count > 0 && clientList[0] != null)
        {
            Client client = clientList[0].GetComponent<Client>();
            if (client != null && client.requestedProduct != null)
            {
                return client.requestedProduct.prefab;
            }
        }
        return null;
    }
    // Fonction à ajouter pour renvoyer le nom du produit au joueur
    public string GetFirstClientProductName()
    {
        if (clientList.Count > 0 && clientList[0] != null)
        {
            Client client = clientList[0].GetComponent<Client>();
            if (client != null)
            {
                // On renvoie le nom du produit stocké dans le script Client
                if (string.IsNullOrEmpty(client.requestedProductName))
                    return "Rien (en attente)";

                return client.requestedProductName;
            }
        }
        return "Aucun client à la caisse";
    }
}