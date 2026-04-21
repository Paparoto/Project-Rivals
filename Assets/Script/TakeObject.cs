using UnityEngine;

public class TakeObject : MonoBehaviour
{
    public GameObject objectToPickup;

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
        heldObject = objectToPickup;
        heldObject.SetActive(false);
    }

    private void Drop()
    {
        Destroy(heldObject);
        heldObject = null;
    }
}