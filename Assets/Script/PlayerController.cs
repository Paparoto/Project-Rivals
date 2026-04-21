using UnityEngine;
using System.Collections;
public class PlayerMovement3D : MonoBehaviour
{
    [Header("Mouvement")]
    public float speed = 40f;

    [Header("Touches clavier")]
    public KeyCode moveUp;
    public TransactionManager transactionManager;
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

    [Header("Zones d'étagères")]
    public GameObject TargetZone1;
    public GameObject TargetZone2;
    public GameObject TargetZone3;
    public GameObject TargetZone4;

    private bool isInZone1, isInZone2, isInZone3, isInZone4;

    [Header("Caisse et Inventaire")]
    public GameObject CashierTargetZone;
    public QueueManager linkedQueue;
    public Transform handTransform; // Objet vide enfant du joueur pour porter l'objet
    private GameObject heldObject;  // L'objet actuellement tenu


    [Header("Interface PC")]
    public GameObject monPanelUI;     // Glisse le Panel UI ici
    public bool isInPCZone = false;  // État de présence devant le PC

    
    private bool isInCashierZone = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();

        axisH = $"joystick {gamepadIndex + 1} axis 0";
        axisV = $"joystick {gamepadIndex + 1} axis 1";

        if (monPanelUI != null) monPanelUI.SetActive(false);
        // --- RECHERCHE AUTOMATIQUE DU BON MANAGER SUR LA CAMERA ---
        // On récupère TOUS les QueueManagers présents sur la Main Camera
        QueueManager[] tousLesManagers = Camera.main.GetComponents<QueueManager>();

        foreach (QueueManager qm in tousLesManagers)
        {
            // Si l'assignedPlayer du script correspond au numéro du joueur (gamepadIndex + 1)
            if (qm.assignedPlayer == (gamepadIndex + 1))
            {
                linkedQueue = qm;
            }
        }
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


        string btnInteract = "joystick " + (gamepadIndex + 1) + " button 1";

        if (Input.GetKeyDown(btnInteract))
        {
            // DEBUG : Apparaît à chaque fois que tu appuies sur le bouton
            Debug.Log("Bouton appuyé par Joueur " + (gamepadIndex + 1));

            if (isInCashierZone)
            {
                GererCaisse();
            }
            else
            {
                // On essaie de ramasser si on n'est pas à la caisse
                TesterRamassageEtagere();
            }
        }
        void GererCaisse()
{
    if (linkedQueue == null) return;

    // CAS 1 : On tient un objet, on essaie de le vendre
    if (heldObject != null)
    {
        string nomVoulu = linkedQueue.GetFirstClientProductName();
        
        // Vérification si l'objet en main est le bon
        if (heldObject.name.Contains(nomVoulu))
        {
            Debug.Log("Vente réussie !");
            
            // Transaction
            TransactionManager.PlayerData pData = (gamepadIndex == 0) ? transactionManager.player1 : transactionManager.player2;
            transactionManager.Sell(pData, linkedQueue.GetFirstClientProduct());

            // On vide la main
            Destroy(heldObject);
            heldObject = null;

            // Le client s'en va
            linkedQueue.ServeClient();
        }
        else
        {
            Debug.Log("Le client ne veut pas ça, il veut : " + nomVoulu);
        }
    }
    // CAS 2 : Main vide, on demande au client ce qu'il veut
    else
    {
        linkedQueue.TriggerFirstClientBubble();
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

    // Détermine quel joueur selon gamepadIndex
    TransactionManager.PlayerData player = gamepadIndex == 0 
        ? transactionManager.player1 
        : transactionManager.player2;

    // Vérifie qu'il a au moins une tomate
    TransactionManager.Product tomate = player.inventory.Find(x => x.name == "Tomate");
    if (tomate == null)
    {
        Debug.Log("Pas de tomate en stock !");
        return;
    }

    // Enlève une tomate de l'inventaire
    player.inventory.Remove(tomate);

    // Lance la tomate
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
        // 1. Détection du PC
        if (PCTargetZone != null && other.gameObject == PCTargetZone)
        {
            isInPCZone = true;
            Debug.Log("Joueur " + (gamepadIndex + 1) + " est devant son PC.");
        }

        // 2. Détection de la Caisse
        if (CashierTargetZone != null && other.gameObject == CashierTargetZone)
        {
            isInCashierZone = true;
            Debug.Log("Joueur " + (gamepadIndex + 1) + " est à la caisse.");
        }

        // 3. Détection des 4 Zones d'étagères
        if (TargetZone1 != null && other.gameObject == TargetZone1)
        {
            isInZone1 = true;
            Debug.Log("Joueur " + (gamepadIndex + 1) + " : Zone 1 (Viande)");
        }
        else if (TargetZone2 != null && other.gameObject == TargetZone2)
        {
            isInZone2 = true;
            Debug.Log("Joueur " + (gamepadIndex + 1) + " : Zone 2 (Divers)");
        }
        else if (TargetZone3 != null && other.gameObject == TargetZone3)
        {
            isInZone3 = true;
            Debug.Log("Joueur " + (gamepadIndex + 1) + " : Zone 3 (Poisson)");
        }
        else if (TargetZone4 != null && other.gameObject == TargetZone4)
        {
            isInZone4 = true;
            Debug.Log("Joueur " + (gamepadIndex + 1) + " : Zone 4 (Legumes)");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Sortie du PC
        if (PCTargetZone != null && other.gameObject == PCTargetZone)
        {
            isInPCZone = false;
            if (monPanelUI != null) monPanelUI.SetActive(false);
            Debug.Log("Joueur " + (gamepadIndex + 1) + " s'est éloigné du PC.");
        }

        // Sortie de la Caisse
        if (CashierTargetZone != null && other.gameObject == CashierTargetZone)
        {
            isInCashierZone = false;
        }

        // Sortie des Zones d'étagères
        if (other.gameObject == TargetZone1) isInZone1 = false;
        if (other.gameObject == TargetZone2) isInZone2 = false;
        if (other.gameObject == TargetZone3) isInZone3 = false;
        if (other.gameObject == TargetZone4) isInZone4 = false;
    }
    void TesterRamassageEtagere()
    {
        if (heldObject != null) return; // On tient déjà quelque chose
        if (linkedQueue == null) return;

        GameObject prefabVoulu = linkedQueue.GetFirstClientRequest();
        string cat = linkedQueue.GetFirstClientCategory();

        if (prefabVoulu == null)
        {
            Debug.Log("Le client n'a pas encore fait de commande.");
            return;
        }

        bool auBonEndroit = false;

        // --- MAPPING DES CATÉGORIES ---
        // Note : attention aux majuscules et au 's' (Legumes vs Legume)
        if (cat.Contains("Viande") && isInZone1) auBonEndroit = true;
        else if (cat.Contains("Divers") && isInZone2) auBonEndroit = true;
        else if (cat.Contains("Poisson") && isInZone3) auBonEndroit = true;
        else if (cat.Contains("Legumes") && isInZone4) auBonEndroit = true;
        // Si tes fruits/desserts sont dans "Divers", ça passera en zone 3

        if (auBonEndroit)
        {
            heldObject = Instantiate(prefabVoulu, handTransform.position, handTransform.rotation);
            heldObject.transform.SetParent(handTransform);
            heldObject.name = prefabVoulu.name; // Nettoie le nom pour la vente

            if (heldObject.GetComponent<Rigidbody>())
                heldObject.GetComponent<Rigidbody>().isKinematic = true;

            Debug.Log("SUCCÈS : Objet ramassé !");
        }
        else
        {
            Debug.Log("ERREUR : Mauvaise zone pour " + cat + " (Zones: Z1=" + isInZone1 + ", Z2=" + isInZone2 + ")");
        }
    }

    // Fonction pour vendre
    void VendreObjetAuClient()
    {
        string nomVoulu = linkedQueue.GetFirstClientProductName();

        // On vérifie si l'objet dans notre main est le bon
        if (heldObject.name.Contains(nomVoulu))
        {
            Debug.Log("Merci ! Vente effectuée.");

            // 1. Calcul de la transaction financière
            TransactionManager.PlayerData pData = (gamepadIndex == 0) ? transactionManager.player1 : transactionManager.player2;
            TransactionManager.Product produitComplet = linkedQueue.GetFirstClientProduct();

            if (transactionManager != null && produitComplet != null)
            {
                transactionManager.Sell(pData, produitComplet); // Ajoute l'argent et retire du stock
            }

            // 2. Détruire l'objet visuel dans la main
            Destroy(heldObject);
            heldObject = null;

            // 3. Faire partir le client et faire avancer la file
            linkedQueue.ServeClient();
        }
        else
        {
            Debug.Log("Le client ne veut pas cet objet ! Il veut : " + nomVoulu);
        }
    }
    void GererCaisse()
    {
        if (linkedQueue == null) return;

        // CAS 1 : On tient un objet, on essaie de le vendre
        if (heldObject != null)
        {
            string nomVoulu = linkedQueue.GetFirstClientProductName();

            // Vérification si l'objet en main est le bon
            if (heldObject.name.Contains(nomVoulu))
            {
                Debug.Log("Vente réussie !");

                // Transaction
                TransactionManager.PlayerData pData = (gamepadIndex == 0) ? transactionManager.player1 : transactionManager.player2;
                transactionManager.Sell(pData, linkedQueue.GetFirstClientProduct());

                // On vide la main
                Destroy(heldObject);
                heldObject = null;

                // Le client s'en va
                linkedQueue.ServeClient();
            }
            else
            {
                Debug.Log("Le client ne veut pas ça, il veut : " + nomVoulu);
            }
        }
        // CAS 2 : Main vide, on demande au client ce qu'il veut
        else
        {
            linkedQueue.TriggerFirstClientBubble();
        }
    }
}