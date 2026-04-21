using UnityEngine;

public class TakeObject : MonoBehaviour
{

    [Header("Lien File d'attente")]
    public QueueManager maFileAttente; // Glisse le QueueManager ici dans l'inspecteur

    // Cette fonction te donnera l'objet voulu en temps réel
    public GameObject GetTargetProduct()
    {
        if (maFileAttente != null)
        {
            return maFileAttente.GetFirstClientRequest();
        }
        return null;
    }

    public GameObject heldObject;
    public KeyCode take;

    private void Update()
    {
        if (Input.GetKeyDown(take))
        {
            if (heldObject == null)
            {
                Pickup();
            }
            else
            {
                Drop();
            }
        }
    }

    private void Pickup()
    {
        heldObject = GetTargetProduct();
        heldObject.SetActive(false);
    }

    private void Drop()
    {
        Destroy(heldObject);
        heldObject = null;
    }

    void OnValidate()
    {
        // Ce code s'exécute dès que la variable change
        if (GetTargetProduct() != null)
        {
            Debug.Log("TakeObject : J'ai bien reçu l'objet " + GetTargetProduct().name);
        }
    }
}