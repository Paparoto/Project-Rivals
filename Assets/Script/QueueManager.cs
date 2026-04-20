using UnityEngine;
using System.Collections.Generic;

public class QueueManager : MonoBehaviour
{
    public GameObject clientPrefab;
    public Transform[] waitPoints;
    public Transform spawnPoint;
    public Transform exitPoint;

    [Header("Réglages Automatiques")]
    public float spawnDelay = 2f;      // Temps avant le 1er client
    public float spawnInterval = 5f;   // Temps entre chaque client (en secondes)

    private List<GameObject> clientList = new List<GameObject>();

    void Start()
    {
        // Cette ligne appelle la fonction "SpawnClient" toutes les 'spawnInterval' secondes
        InvokeRepeating("SpawnClient", spawnDelay, spawnInterval);
    }

    void Update()
    {
        // On garde la touche M pour faire partir le client de devant
        if (Input.GetKeyDown(KeyCode.M))
        {
            ServeClient();
        }
    }

    public void SpawnClient()
    {
        // On vérifie s'il y a de la place dans la file (max 3)
        if (clientList.Count < waitPoints.Length)
        {
            GameObject newClient = Instantiate(clientPrefab, spawnPoint.position, Quaternion.identity);
            clientList.Add(newClient);
            UpdateQueuePositions();
        }
    }

    void ServeClient()
    {
        if (clientList.Count > 0)
        {
            GameObject clientToLeave = clientList[0];
            clientList.RemoveAt(0);
            clientToLeave.GetComponent<Client>().Leave(exitPoint.position);

            UpdateQueuePositions();
        }
    }

    void UpdateQueuePositions()
    {
        for (int i = 0; i < clientList.Count; i++)
        {
            clientList[i].GetComponent<Client>().SetTarget(waitPoints[i].position);
        }
    }
}