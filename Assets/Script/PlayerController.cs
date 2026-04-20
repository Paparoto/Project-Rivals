using UnityEngine;

public class PlayerMovement3D : MonoBehaviour
{
    public float speed = 5f;

    public KeyCode moveUp;
    public KeyCode moveDown;
    public KeyCode moveLeft;
    public KeyCode moveRight;

    private Rigidbody rb;
    private Vector3 movement;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        movement = Vector3.zero;

        if (Input.GetKey(moveUp)) movement += Vector3.forward;
        if (Input.GetKey(moveDown)) movement += Vector3.back;
        if (Input.GetKey(moveLeft)) movement += Vector3.left;
        if (Input.GetKey(moveRight)) movement += Vector3.right;

        movement = movement.normalized;

        Vector3 velocity = movement * speed;
        velocity.y = rb.linearVelocity.y; // garde la gravité

        rb.linearVelocity = velocity;
    }
}