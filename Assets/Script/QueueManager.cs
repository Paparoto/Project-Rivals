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
    private BonusManager bonusManager;
    

    void Start()
    {
        bonusManager = FindObjectOfType<BonusManager>();
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
    if (assignedPlayer == 1){Invoke("SpawnClient", Random.Range(minInterval * bonusManager.P1clientBonus, maxInterval * bonusManager.P1clientBonus));}
    else if (assignedPlayer == 2){Invoke("SpawnClient", Random.Range(minInterval * bonusManager.P2clientBonus, maxInterval * bonusManager.P2clientBonus));}

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

            // Dans QueueManager.cs
            if (i == 0)
            {
                client.SetAsFirstInQueue();
                TransactionManager.PlayerData pData = (assignedPlayer == 1) ? transactionManager.player1 : transactionManager.player2;

                if (pData.inventory.Count > 0)
                {
                    client.Request(pData); // Le client ne changera pas si hasChosenProduct est true
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
    public void TriggerFirstClientBubble()
    {
        if (clientList.Count > 0 && clientList[0] != null)
        {
            Client client = clientList[0].GetComponent<Client>();
            if (client != null)
            {
                client.AfficherMaBulle();
            }
        }
    }

    // 3. Pour savoir à quelle étagère aller
    public string GetFirstClientCategory()
    {
        if (clientList.Count > 0 && clientList[0] != null) return clientList[0].GetComponent<Client>().requestedProductCategory;
        return "";
    }

    // 4. Pour la vente finale (TransactionManager)
    public TransactionManager.Product GetFirstClientProduct()
    {
        if (clientList.Count > 0 && clientList[0] != null) return clientList[0].GetComponent<Client>().requestedProduct;
        return null;
    }
}