using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlayerMovement3D : MonoBehaviour
{
    [Header("Mouvement")]
    public float speed = 40f;
    public TransactionManager transactionManager;
    public KeyCode moveUp;
    public KeyCode moveDown;
    public KeyCode moveLeft;
    public KeyCode moveRight;

    [Header("Manette")]
    public int gamepadIndex = 0; // 0 = joueur 1, 1 = joueur 2

    [Header("Lancer")]
    public GameObject throwPrefab;
    public float throwSpeed = 10f;
    public Transform throwOrigin;
    public float throwCooldown = 2f;
    private float throwTimer = 0f;
    private bool canThrow = true;

    [Header("Zones d'interaction")]
    public GameObject PCTargetZone;
    public GameObject CashierTargetZone;
    public GameObject TargetZone1;
    public GameObject TargetZone2;
    public GameObject TargetZone3;
    public GameObject TargetZone4;

    [Header("Interface et Inventaire")]
    public GameObject monPanelUI;
    public Transform handTransform;
    public QueueManager linkedQueue;

    private Rigidbody rb;
    private Animator animator;
    private Vector3 movement;
    private string axisH;
    private string axisV;
    private GameObject heldObject;

    // États des zones
    [HideInInspector] public bool isInPCZone = false;
    private bool isInCashierZone = false;
    private bool isInZone1, isInZone2, isInZone3, isInZone4;
    private bool isStunned = false;

    [HideInInspector] public string carriedProductName = ""; // Nom de l'objet en main

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();

        axisH = $"joystick {gamepadIndex + 1} axis 0";
        axisV = $"joystick {gamepadIndex + 1} axis 1";

        if (monPanelUI != null) monPanelUI.SetActive(false);

        // Recherche automatique du manager
        QueueManager[] tousLesManagers = Camera.main.GetComponents<QueueManager>();
        foreach (QueueManager qm in tousLesManagers)
        {
            if (qm.assignedPlayer == (gamepadIndex + 1)) linkedQueue = qm;
        }
    }

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
        if (isStunned) return;

        // --- MOUVEMENT ---
        float h = Input.GetAxisRaw(axisH);
        float v = Input.GetAxisRaw(axisV);
        movement = Vector3.zero;

        bool usingGamepad = Mathf.Abs(h) > 0.2f || Mathf.Abs(v) > 0.2f;
        if (usingGamepad)
        {
            movement = new Vector3(h, 0, v).normalized;
            float angle = Mathf.Atan2(movement.x, movement.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, angle, 0);
        }

        Vector3 keyboardMovement = Vector3.zero;
        if (Input.GetKey(moveUp)) keyboardMovement += Vector3.forward;
        else if (Input.GetKey(moveDown)) keyboardMovement += Vector3.back;
        else if (Input.GetKey(moveLeft)) keyboardMovement += Vector3.left;
        else if (Input.GetKey(moveRight)) keyboardMovement += Vector3.right;

        if (keyboardMovement != Vector3.zero)
        {
            movement = keyboardMovement.normalized;
            transform.rotation = Quaternion.LookRotation(movement);
        }

        animator.SetBool("isRunning2", movement != Vector3.zero);

        HandleThrow();

        // --- INTERACTIONS (BOUTON 1) ---
        string btnInteract = "joystick " + (gamepadIndex + 1) + " button 1";

        if (Input.GetKeyDown(btnInteract))
        {
            if (isInPCZone)
            {
                if (monPanelUI != null) monPanelUI.SetActive(!monPanelUI.activeSelf);
            }
            else if (isInCashierZone)
            {
                GererCaisse();
            }
            else
            {
                TesterRamassageEtagere();
            }
        }
    }

    void GererCaisse()
    {
        if (linkedQueue == null) return;

        if (heldObject != null)
        {
            string nomVoulu = linkedQueue.GetFirstClientProductName();
            if (heldObject.name.Contains(nomVoulu))
            {
                Debug.Log("Vente réussie !");
                TransactionManager.PlayerData pData = (gamepadIndex == 0) ? transactionManager.player1 : transactionManager.player2;
                transactionManager.Sell(pData, linkedQueue.GetFirstClientProduct());

                Destroy(heldObject);
                heldObject = null;

                // ON VIDE LE NOM AVANT DE RAFRAICHIR
                carriedProductName = "";

                linkedQueue.ServeClient();
                RefreshAllShelves();
            }
            else
            {
                Debug.Log("Mauvais objet ! Il veut : " + nomVoulu);
            }
        }
        else
        {
            linkedQueue.TriggerFirstClientBubble();
        }
    }

    void TesterRamassageEtagere()
    {
        if (heldObject != null || linkedQueue == null) return;

        GameObject prefabVoulu = linkedQueue.GetFirstClientRequest();
        string cat = linkedQueue.GetFirstClientCategory();
        if (prefabVoulu == null) return;

        bool auBonEndroit = false;
        // Mapping Final
        if (cat == "Viande" && isInZone1) auBonEndroit = true;
        else if (cat == "Divers" && isInZone2) auBonEndroit = true;
        else if (cat == "Poisson" && isInZone3) auBonEndroit = true;
        else if (cat == "Legumes" && isInZone4) auBonEndroit = true;

        if (auBonEndroit)
        {
            heldObject = Instantiate(prefabVoulu, handTransform.position, handTransform.rotation);
            heldObject.transform.SetParent(handTransform);

            // ON STOCKE LE NOM PROPRE ICI
            carriedProductName = prefabVoulu.name;
            heldObject.name = prefabVoulu.name;

            if (heldObject.GetComponent<Rigidbody>()) heldObject.GetComponent<Rigidbody>().isKinematic = true;

            // RAFRAICHIR TOUTES LES ÉTAGÈRES
            RefreshAllShelves();
        }

    }

    void HandleThrow()
    {
        bool throwPressed = (gamepadIndex == 0) ?
            (Input.GetKey("joystick 1 button 5") || Input.GetKey(KeyCode.RightControl)) :
            (Input.GetKey("joystick 2 button 5") || Input.GetKey(KeyCode.Q));

        if (!canThrow)
        {
            throwTimer -= Time.deltaTime;
            if (throwTimer <= 0f) canThrow = true;
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
        TransactionManager.PlayerData player = gamepadIndex == 0 ? transactionManager.player1 : transactionManager.player2;
        TransactionManager.Product tomate = player.inventory.Find(x => x.name == "Tomate");

        if (tomate == null) return;

        player.inventory.Remove(tomate);
        Vector3 spawnPos = throwOrigin != null ? throwOrigin.position : transform.position + transform.forward;
        GameObject thrown = Instantiate(throwPrefab, spawnPos, transform.rotation);
        if (thrown.GetComponent<Rigidbody>()) thrown.GetComponent<Rigidbody>().linearVelocity = transform.forward * throwSpeed;
    }

    void FixedUpdate()
    {
        Vector3 velocity = movement * speed;
        velocity.y = rb.linearVelocity.y;
        rb.linearVelocity = velocity;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == PCTargetZone) isInPCZone = true;
        if (other.gameObject == CashierTargetZone) isInCashierZone = true;
        if (other.gameObject == TargetZone1) isInZone1 = true;
        if (other.gameObject == TargetZone2) isInZone2 = true;
        if (other.gameObject == TargetZone3) isInZone3 = true;
        if (other.gameObject == TargetZone4) isInZone4 = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == PCTargetZone) { isInPCZone = false; if (monPanelUI != null) monPanelUI.SetActive(false); }
        if (other.gameObject == CashierTargetZone) isInCashierZone = false;
        if (other.gameObject == TargetZone1) isInZone1 = false;
        if (other.gameObject == TargetZone2) isInZone2 = false;
        if (other.gameObject == TargetZone3) isInZone3 = false;
        if (other.gameObject == TargetZone4) isInZone4 = false;
    }
    public string GetHeldObjectName()
    {
        if (heldObject == null) return "";
        // On renvoie le nom de l'objet (qui a été nettoyé lors du ramassage)
        return heldObject.name;
    }
    void RefreshAllShelves()
    {
        FoodVisualizer[] visualizers = FindObjectsByType<FoodVisualizer>(FindObjectsSortMode.None);
        foreach (var v in visualizers) v.Refresh();
    }
}   