using UnityEngine;

public class BonusManager : MonoBehaviour
{
    private const float MIN_BONUS = 0.3f;

    private float _P1speedBonus = 1f;
    private float _P1clientBonus = 1f;
    private float _P1moneyBonus = 1f;
    private float _P2speedBonus = 1f;
    private float _P2clientBonus = 1f;
    private float _P2moneyBonus = 1f;

    public float P1speedBonus  { get => _P1speedBonus;  set => _P1speedBonus  = Mathf.Max(MIN_BONUS, value); }
    public float P1clientBonus { get => _P1clientBonus; set => _P1clientBonus = Mathf.Max(MIN_BONUS, value); }
    public float P1moneyBonus  { get => _P1moneyBonus;  set => _P1moneyBonus  = Mathf.Max(MIN_BONUS, value); }
    public float P2speedBonus  { get => _P2speedBonus;  set => _P2speedBonus  = Mathf.Max(MIN_BONUS, value); }
    public float P2clientBonus { get => _P2clientBonus; set => _P2clientBonus = Mathf.Max(MIN_BONUS, value); }
    public float P2moneyBonus  { get => _P2moneyBonus;  set => _P2moneyBonus  = Mathf.Max(MIN_BONUS, value); }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
