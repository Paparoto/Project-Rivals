using UnityEngine;

public class Tomato : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        PlayerMovement3D player = collision.gameObject.GetComponent<PlayerMovement3D>();
        if (player != null)
        {
            player.Stun(5f);
        }

        Destroy(gameObject);
    }
}