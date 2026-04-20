using UnityEngine;

public class PlayerMovement3D : MonoBehaviour
{
    public float speed = 5f;

    public KeyCode moveUp;
    public KeyCode moveDown;
    public KeyCode moveLeft;
    public KeyCode moveRight;

    private Rigidbody rb;
    private Animator animator;
    private Vector3 movement;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
    }

    void FixedUpdate()
    {
        movement = Vector3.zero;

        if (Input.GetKey(moveUp))
        {
            movement += Vector3.forward;
            transform.rotation = Quaternion.Euler(0, 0, 0);   // face caméra
        }
        else if (Input.GetKey(moveDown))
        {
            movement += Vector3.back;
            transform.rotation = Quaternion.Euler(0, 180, 0); // dos caméra
        }
        else if (Input.GetKey(moveLeft))
        {
            movement += Vector3.left;
            transform.rotation = Quaternion.Euler(0, 270, 0); // gauche
        }
        else if (Input.GetKey(moveRight))
        {
            movement += Vector3.right;
            transform.rotation = Quaternion.Euler(0, 90, 0);  // droite
        }

        if (movement != Vector3.zero)
            animator.SetBool("isWalking", true);
        else
            animator.SetBool("isWalking", false);

        movement = movement.normalized;

        Vector3 velocity = movement * speed;
        velocity.y = rb.linearVelocity.y;

        rb.linearVelocity = velocity;
    }
}