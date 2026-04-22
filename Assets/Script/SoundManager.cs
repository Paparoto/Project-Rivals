using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Sons")]
    public AudioClip saleSound;

    private AudioSource audioSource;

    void Awake()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void PlaySaleSound()
    {
        if (saleSound != null)
            audioSource.PlayOneShot(saleSound);
    }
}