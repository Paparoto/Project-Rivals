using UnityEngine;

public class Client : MonoBehaviour
{
    public float speed = 5f;
    private Vector3 targetPosition;
    private bool isLeaving = false;

    void Update()
    {
        // Déplacement fluide vers la cible
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // Si le client est en train de partir et a atteint sa destination de sortie, on le détruit
        if (isLeaving && Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            Destroy(gameObject);
        }
    }

    public void SetTarget(Vector3 newPos)
    {
        targetPosition = newPos;
    }

    public void Leave(Vector3 exitPos)
    {
        isLeaving = true;
        targetPosition = exitPos;
    }
}