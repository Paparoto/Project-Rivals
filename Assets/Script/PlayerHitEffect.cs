using UnityEngine;
using UnityEngine.UI;

public class PlayerHitEffect : MonoBehaviour
{
    public Image splatImage; // glisse ton image rouge dans l'Inspector
    public float splatDuration = 2f;

    private float timer = 0f;
    private bool showing = false;

    void Update()
    {
        if (showing)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                splatImage.gameObject.SetActive(false);
                showing = false;
            }
        }
    }

    public void ShowSplat()
    {
        splatImage.gameObject.SetActive(true);
        timer = splatDuration;
        showing = true;
    }
}