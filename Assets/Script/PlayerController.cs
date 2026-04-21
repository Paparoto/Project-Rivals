using UnityEngine;
using System.Collections;
public class PlayerMovement3D : MonoBehaviour
{
    [Header("Mouvement")]
    public float speed = 40f;

    [Header("Touches clavier")]
    public KeyCode moveUp;
    public KeyCode moveDown;
    public KeyCode moveLeft;
    public KeyCode moveRight;

    [Header("Manette")]
    public int gamepadIndex = 0; // 0 = joueur 1, 1 = joueur 2

    [Header("Lancer")]
    public GameObject throwPrefab;
    public float throwSpeed = 10f;
    public Transform throwOrigin; // point de départ du lancer (optionnel, sinon utilise la position du joueur)

    private Rigidbody rb;
    private Animator animator;
    private Vector3 movement;

    private string axisH;
    private string axisV;

    private bool canThrow = true; // évite de lancer en boucle si la gâchette reste enfoncée

    [Header("Zones d'interaction")]
    public GameObject PCTargetZone;   // Glisse le PC ici (PC Gauche pour J1, PC Droite pour J2)
    public GameObject PoissonZone;    // Glisse l'étagère Poisson
    public GameObject ViandeZone;     // Glisse l'étagère Viande
    public GameObject SucreZone;      // Glisse l'étagère Sucré

    [Header("Interface PC")]
    public GameObject monPanelUI;     // Glisse le Panel UI ici
    private bool isInPCZone = false;  // État de présence devant le PC

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();

        axisH = $"joystick {gamepadIndex + 1} axis 0";
        axisV = $"joystick {gamepadIndex + 1} axis 1";

        if (monPanelUI != null) monPanelUI.SetActive(false);
    }
    private bool isStunned = false;

    // Ajoute cette méthode
    public void Stun(float duration)
    {
        StopAllCoroutines(); 
        StartCoroutine(StunCoroutine(duration));
    }

    IEnumerator StunCoroutine(float duration)
    {
        isStunned = true;
        animator.SetBool("isRunning2", false);
        rb.linearVelocity = Vector3.zero;
        yield return new WaitForSeconds(duration);
        isStunned = false;
    }
    void Update()
    {
        
        float h2 = Input.GetAxisRaw("joystick 2 axis 0");
        float v2 = Input.GetAxisRaw("joystick 2 axis 1");


        movement = Vector3.zero;
        if (isStunned) return;
        float h = 0f;
        float v = 0f;


        h = Input.GetAxisRaw(axisH);
        v = Input.GetAxisRaw(axisV);
        

        bool usingGamepad = Mathf.Abs(h) > 0.2f || Mathf.Abs(v) > 0.2f;

        if (usingGamepad)
        {
            movement = new Vector3(h, 0, v).normalized;
            if (movement != Vector3.zero)
            {
                float angle = Mathf.Atan2(movement.x, movement.z) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, angle, 0);
            }
        }

        // === CLAVIER ===
        Vector3 keyboardMovement = Vector3.zero;
        if (Input.GetKey(moveUp)) keyboardMovement += Vector3.forward;
        else if (Input.GetKey(moveDown)) keyboardMovement += Vector3.back;
        else if (Input.GetKey(moveLeft)) keyboardMovement += Vector3.left;
        else if (Input.GetKey(moveRight)) keyboardMovement += Vector3.right;

        if (keyboardMovement != Vector3.zero)
        {
            movement = keyboardMovement.normalized;
            if (Input.GetKey(moveUp)) transform.rotation = Quaternion.Euler(0, 0, 0);
            else if (Input.GetKey(moveDown)) transform.rotation = Quaternion.Euler(0, 180, 0);
            else if (Input.GetKey(moveLeft)) transform.rotation = Quaternion.Euler(0, 270, 0);
            else if (Input.GetKey(moveRight)) transform.rotation = Quaternion.Euler(0, 90, 0);
        }

        animator.SetBool("isRunning2", movement != Vector3.zero);

        // === LANCER (gâchette droite = axis 5 sur Xbox) ===
        HandleThrow();
        // === INTERACTION SPECIAL ===
        // Vérifie si le joueur est devant son PC
        if (isInPCZone)
        {
            // Utilise Button 1 pour allumer/éteindre le panel
            string interactionButton = "joystick " + (gamepadIndex + 1) + " button 1";

            if (Input.GetKeyDown(interactionButton))
            {
                if (monPanelUI != null)
                {
                    // Allume si éteint, éteint si allumé
                    monPanelUI.SetActive(!monPanelUI.activeSelf);
                    Debug.Log("PC Joueur " + (gamepadIndex + 1) + " : Panel Toggled");
                }
            }
        }
    }

public float throwCooldown = 2f; // modifiable dans l'Inspector
private float throwTimer = 0f;

void HandleThrow()
{
    bool throwPressed = false;

    if (gamepadIndex == 0)
    {
        throwPressed = Input.GetKey("joystick 1 button 5") || Input.GetKey(KeyCode.RightControl);
    }
    else
    {
        throwPressed = Input.GetKey("joystick 2 button 5") || Input.GetKey(KeyCode.Q);
    }

    // Décompte du cooldown
    if (!canThrow)
    {
        throwTimer -= Time.deltaTime;
        if (throwTimer <= 0f)
            canThrow = true;
    }

    if (throwPressed && canThrow)
    {
        canThrow = false;
        throwTimer = throwCooldown;
        animator.SetTrigger("throw");
        ThrowObject();
    }
}

    void ThrowObject()
    {
        if (throwPrefab == null) return;

        Vector3 spawnPos = throwOrigin != null ? throwOrigin.position : transform.position + transform.forward;

        GameObject thrown = Instantiate(throwPrefab, spawnPos, transform.rotation);

        Rigidbody thrownRb = thrown.GetComponent<Rigidbody>();
        if (thrownRb != null)
        {
            thrownRb.linearVelocity = transform.forward * throwSpeed;
        }
    }

    void FixedUpdate()
    {
        Vector3 velocity = movement * speed;
        velocity.y = rb.linearVelocity.y;
        rb.linearVelocity = velocity;
    }
    private void OnTriggerEnter(Collider other)
    {
        // 1. Détection du PC (pour activer l'interaction plus tard dans Update)
        if (PCTargetZone != null && other.gameObject == PCTargetZone)
        {
            isInPCZone = true;
            Debug.Log("Joueur " + (gamepadIndex + 1) + " est devant son PC. Appuyez sur Bouton 1 pour le panel.");
        }

        // 2. Détection Étagère Poisson
        if (PoissonZone != null && other.gameObject == PoissonZone)
        {
            Debug.Log("Message le joueur " + (gamepadIndex + 1) + " : Voici le rayon POISSON 🐟");
        }

        // 3. Détection Étagère Viande
        if (ViandeZone != null && other.gameObject == ViandeZone)
        {
            Debug.Log("Message le joueur " + (gamepadIndex + 1) + ": Voici le rayon VIANDE 🥩");
        }

        // 4. Détection Étagère Sucré
        if (SucreZone != null && other.gameObject == SucreZone)
        {
            Debug.Log("Message le joueur " + (gamepadIndex + 1) + ": Voici le rayon SUCRÉ 🍬");
        }
    }


    private void OnTriggerExit(Collider other)
    {
        // Quand on s'éloigne du PC, on désactive la possibilité d'appuyer sur le bouton
        if (PCTargetZone != null && other.gameObject == PCTargetZone)
        {
            isInPCZone = false;

            // Optionnel : On peut aussi éteindre le panel automatiquement quand il part
            if (monPanelUI != null) monPanelUI.SetActive(false);

            Debug.Log("Joueur " + (gamepadIndex + 1) + " s'est éloigné du PC.");
        }
    }
}