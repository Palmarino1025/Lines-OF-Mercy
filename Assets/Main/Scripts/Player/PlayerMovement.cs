using UnityEngine;

// This makes sure a CharacterController exists on the same GameObject
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Speeds")]
    public float walkSpeed = 4f;
    public float sprintSpeed = 7f;

    [Header("Jump / Gravity")]
    public float jumpHeight = 1.5f;
    public float gravityForce = -9.81f; // Negative because gravity pulls down

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
            Debug.LogError("PlayerMovement: Please assign the cameraTransform in the inspector.");
        }
    }

    void Update()
    {
        bool isGrounded = characterController.isGrounded;

        // Small downward force to keep the player "snapped" to the ground
        if (isGrounded && playerVelocity.y < 0f)
        {
            playerVelocity.y = -2f;
        }

        // If movement is locked (dialogue open), we ignore WASD
        float inputHorizontal = 0f;
        float inputVertical = 0f;

        if (!isMovementLocked)
        {
            inputHorizontal = Input.GetAxis("Horizontal"); // A / D
            inputVertical = Input.GetAxis("Vertical");     // W / S
        }

        // Get camera-relative directions and flatten them
        Vector3 forwardDirection = cameraTransform.forward;
        Vector3 rightDirection = cameraTransform.right;

        forwardDirection.y = 0f;
        rightDirection.y = 0f;

        forwardDirection.Normalize();
        rightDirection.Normalize();

        Vector3 moveDirection = rightDirection * inputHorizontal + forwardDirection * inputVertical;

        if (moveDirection.magnitude > 1f)
        {
            moveDirection.Normalize();
        }

        // Decide if we are walking or sprinting
        float currentSpeed = walkSpeed;
        if (!isMovementLocked && Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = sprintSpeed;
        }

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

        // Apply vertical motion
        characterController.Move(playerVelocity * Time.deltaTime);
    }

    // Called by other scripts (like dialogue) to freeze/unfreeze movement
    public void SetMovementLock(bool shouldLockMovement)
    {
        isMovementLocked = shouldLockMovement;

        // stop all horizontal motion instantly
        if (shouldLockMovement)
        {
            playerVelocity.x = 0f;
            playerVelocity.z = 0f;
        }
    }
}
