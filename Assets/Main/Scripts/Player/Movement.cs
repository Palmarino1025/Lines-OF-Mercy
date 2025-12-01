using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float moveSpeed = 5f;        // Movement speed
    public float jumpForce = 7f;        // Jump Force
    public LayerMask groundLayer;

    private Rigidbody rb;               // Reference to Rigidbody
    private Vector3 movement;
    private bool isGrounded = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Get the Rigidbody component
    }

    // Update is called once per frame
    void Update()
    {
        // Get WASD movement input
        float moveX = Input.GetAxisRaw("Horizontal");  // A/D or Left/Right
        float moveZ = Input.GetAxisRaw("Vertical");    // W/S or Up/Down

        movement = new Vector3(moveX, 0f, moveZ).normalized;

        CheckGround();

        // Jump input
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    void CheckGround()
    {
        // Raycast straight down from the player's position
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f, groundLayer);
    }
}