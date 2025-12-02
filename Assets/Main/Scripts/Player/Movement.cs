using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float moveSpeed = 5f;        // Movement speed
    public float jumpForce = 3f;        // Jump Force
    public float jumpCooldown = 1.5f;   // One and a half second between jumps

    private Rigidbody rb;               // Reference to Rigidbody
    private Vector3 movement;
    private float jumpTimer = 0f;

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

        // Countdown for jump cooldown
        if (jumpTimer > 0f)
            jumpTimer -= Time.deltaTime;

        // Jump input
        if (Input.GetKeyDown(KeyCode.Space) && jumpTimer <= 0f)
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
        jumpTimer = jumpCooldown;    // reset cooldown
    }
}