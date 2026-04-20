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

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();

        axisH = $"joystick {gamepadIndex + 1} axis 0";
        axisV = $"joystick {gamepadIndex + 1} axis 1";
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
        movement = Vector3.zero;
        if (isStunned) return;
        // === MANETTE ===
        float h = 0f;
        float v = 0f;

        if (gamepadIndex == 0)
        {
            h = Input.GetAxisRaw(axisH);
            v = Input.GetAxisRaw(axisV);
        }

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
}