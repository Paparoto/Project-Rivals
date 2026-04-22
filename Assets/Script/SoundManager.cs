using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Sons")]
    public AudioClip saleSound;

    [Header("Musique de fond")]
    public AudioClip backgroundMusic;
    [Range(0f, 1f)] public float musicVolume = 0.5f;

    private AudioSource sfxSource;
    private AudioSource musicSource;

    void Awake()
    {
        Instance = this;

        // Source pour les effets sonores
        sfxSource = gameObject.AddComponent<AudioSource>();

        // Source dédiée à la musique
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.clip = backgroundMusic;
        musicSource.loop = true;
        musicSource.volume = musicVolume;
        musicSource.playOnAwake = false;
    }

    void Start()
    {
        if (backgroundMusic != null)
            musicSource.Play();
    }

    public void PlaySaleSound()
    {
        if (saleSound != null)
            sfxSource.PlayOneShot(saleSound);
    }

    // Optionnel : contrôler la musique depuis d'autres scripts
    public void StopMusic() => musicSource.Stop();
    public void PauseMusic() => musicSource.Pause();
    public void ResumeMusic() => musicSource.UnPause();
    public void SetMusicVolume(float volume) => musicSource.volume = volume;
}