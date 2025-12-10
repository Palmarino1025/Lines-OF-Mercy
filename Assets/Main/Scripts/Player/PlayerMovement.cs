using UnityEngine;

// This makes sure a CharacterController exists on the same GameObject
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public Transform cameraTransform;  // assign main camera in Inspector

    [Header("Gravity / Jump")]
    public float gravity = -9.81f;     // base gravity (negative). Use -9.81 for earth-like.
    public float gravityScale = 2f;    // tune to make falls faster/slower
    private Vector3 velocity;          // vertical velocity stored here

    [Header("Ground Check (optional)")]
    public bool useGroundCheck = false; // if true uses custom ground check, otherwise uses controller.isGrounded
    public Transform groundCheck;       // small transform at feet; assign in Inspector if useGroundCheck = true
    public float groundDistance = 0.2f; // sphere radius for ground check
    public LayerMask groundMask;        // which layers count as ground

    [Header("Jump (optional)")]
    public float jumpHeight = 1.5f;     // jump height in meters
    public KeyCode jumpKey = KeyCode.Space;

    private CharacterController controller;

    [Header("References")]
    public Transform cameraTransform;  // Drag your Main Camera here in the Inspector

    private CharacterController characterController;
    private Vector3 playerVelocity;
    private bool isMovementLocked = false;

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        if (cameraTransform == null)
        {
            Debug.LogError("Please assign the cameraTransform in the inspector.");
        }
    }

    void Update()
    {
        // Get input
        float x = Input.GetAxis("Horizontal"); // A/D
        float z = Input.GetAxis("Vertical");   // W/S

        // Get camera relative directions (flatten Y axis)
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        // Move on the XZ plane
        characterController.Move(moveDirection * currentSpeed * Time.deltaTime);

        // Jump (only if movement is not locked)
        if (!isMovementLocked && Input.GetButtonDown("Jump") && isGrounded)
        {
            // Standard jump formula: v = sqrt(h * -2 * g)
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravityForce);
        }

        // Apply gravity to our vertical velocity
        playerVelocity.y += gravityForce * Time.deltaTime;

        // Calculate movement direction
        Vector3 move = right * x + forward * z;

        if (move.magnitude > 1f)
            move.Normalize();

        // Move the player
        controller.Move(move * moveSpeed * Time.deltaTime);
    }
}
