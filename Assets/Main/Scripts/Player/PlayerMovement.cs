using UnityEngine;

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

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (cameraTransform == null)
            Debug.LogError("Please assign the cameraTransform in the inspector.");
        if (useGroundCheck && groundCheck == null)
            Debug.LogWarning("useGroundCheck is true but groundCheck transform is not assigned. Falling back to controller.isGrounded.");
    }

    void Update()
    {
        // ----- HORIZONTAL MOVEMENT (your existing camera-relative movement) -----
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 move = right * x + forward * z;
        if (move.magnitude > 1f) move.Normalize();

        // ----- GROUND CHECK -----
        bool grounded = false;
        if (useGroundCheck && groundCheck != null)
        {
            grounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        }
        else
        {
            grounded = controller.isGrounded;
        }

        // ----- GRAVITY -----
        // If grounded and falling, set to small negative to keep grounded
        if (grounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }

        // integrate gravity (gravity is negative)
        velocity.y += gravity * gravityScale * Time.deltaTime;

        // ----- JUMP (optional) -----
        if (grounded && Input.GetKeyDown(jumpKey))
        {
            // compute initial velocity to reach jumpHeight: v = sqrt(2 * -gravity * jumpHeight)
            // note: gravity must be negative (e.g. -9.81)
            float effectiveGravity = gravity * gravityScale;
            if (effectiveGravity == 0f) effectiveGravity = -9.81f * gravityScale;
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * effectiveGravity);
        }

        // ----- APPLY MOVEMENT -----
        // Combine horizontal (move * moveSpeed) with vertical velocity
        Vector3 combined = move * moveSpeed + new Vector3(0f, velocity.y, 0f);

        controller.Move(combined * Time.deltaTime);
    }

    // debug visualization for ground check
    void OnDrawGizmosSelected()
    {
        if (useGroundCheck && groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
    }
}
