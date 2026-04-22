using UnityEngine;

public class BonusManager : MonoBehaviour
{
    private const float MIN_BONUS = 0.3f;

    public float P1speedBonus = 1f;
    public float P1clientBonus = 1f;
    public float P1moneyBonus = 1f;
    public float P1patienceBonus = 1f;
    public float P2speedBonus = 1f;
    public float P2clientBonus = 1f;
    public float P2moneyBonus = 1f;
    public float P2patienceBonus = 1f;


    void Start()
    {
        
    }

    void Update()
    {
        if (P1speedBonus <= 0.3f) P1speedBonus=0.3f;
        if (P2speedBonus <= 0.3f) P2speedBonus=0.3f;
        if (P1clientBonus <= 0.3f) P1clientBonus=0.3f;
        if (P2clientBonus <= 0.3f) P2clientBonus=0.3f;
        if (P1moneyBonus <= 0.3f) P1moneyBonus=0.3f;
        if (P2moneyBonus <= 0.3f) P2moneyBonus=0.3f;
        if (P2patienceBonus <= 0.3f) P2patienceBonus=0.3f;
    }
}
