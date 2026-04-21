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
        Client client = newClient.GetComponent<Client>();
        client.transactionManager = transactionManager;
        client.exitPoint = exitPoint;
        client.assignedPlayer = assignedPlayer;
        client.queueManager = this; // ← après la création du client
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
            clientList.RemoveAt(0); // ← on retire de la liste AVANT de le faire partir
            
            if (clientToLeave != null) // ← vérification avant d'y accéder
                clientToLeave.GetComponent<Client>().Leave(exitPoint.position);

            UpdateQueuePositions();
        }
    }

    void UpdateQueuePositions()
    {
        for (int i = clientList.Count - 1; i >= 0; i--)
        {
            // Nettoie la liste si un client a été détruit de façon inattendue
            if (clientList[i] == null)
            {
                clientList.RemoveAt(i);
                continue;
            }

            Client client = clientList[i].GetComponent<Client>();
            client.SetTarget(waitPoints[i].position);

            if (i == 0)
                client.SetAsFirstInQueue();
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