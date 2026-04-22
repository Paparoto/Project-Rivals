using UnityEngine;

public class Tomato : MonoBehaviour
{
    [Header("Son")]
    public AudioClip throwSound;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        if (throwSound != null)
            audioSource.PlayOneShot(throwSound);
    }

    void OnCollisionEnter(Collision collision)
    {
        PlayerMovement3D player = collision.gameObject.GetComponent<PlayerMovement3D>();
        if (player != null)
            player.Stun(5f);

        Destroy(gameObject);
    }
}