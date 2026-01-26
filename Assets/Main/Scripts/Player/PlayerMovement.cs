using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float sprintMultiplier = 1.7f;
    public KeyCode sprintKey = KeyCode.LeftShift;

    public Transform cameraTransform;

    [Header("Gravity / Jump")]
    public float gravity = -9.81f;
    public float gravityScale = 2f;
    private Vector3 playerVelocity;

    [Header("Ground Check (optional)")]
    public bool useGroundCheck = false;
    public Transform groundCheck;
    public float groundDistance = 0.2f;
    public LayerMask groundMask;

    [Header("Jump (optional)")]
    public float jumpHeight = 1.5f;
    public KeyCode jumpKey = KeyCode.Space;

    private CharacterController controller;
    private bool isMovementLocked = false;

    //------------------
    // Exposed State for UI
    //------------------
    public bool IsSprinting { get; private set; }
    public bool IsMoving { get; private set; }

    //------------------
    //  Stamina Hookup
    //------------------
    [Header("Stamina (optional)")]
    public StaminaUI staminaUI;

    [Tooltip("If stamina hits 0, sprint is blocked until stamina recovers to this % (0-1). Example: 0.2 = 20%")]
    [Range(0f, 1f)]
    public float sprintReenableThreshold = 0.2f;

    private bool sprintBlocked = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (cameraTransform == null)
            Debug.LogError("Assign the Camera Transform to PlayerMovement.");

        //auto-find if not assigned
        if (staminaUI == null)
            staminaUI = FindObjectOfType<StaminaUI>();
    }

    void Update()
    {
        bool isGrounded = useGroundCheck ?
            Physics.CheckSphere(groundCheck.position, groundDistance, groundMask) :
            controller.isGrounded;

        if (isGrounded && playerVelocity.y < 0)
            playerVelocity.y = -2f;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 forward = cameraTransform.forward; forward.y = 0;
        Vector3 right = cameraTransform.right; right.y = 0;

        Vector3 move = (right * x + forward * z).normalized;

        float currentSpeed = moveSpeed;

        IsMoving = move.magnitude > 0.1f;

        // ---------------------
        // Sprint + Stamina Logic
        // ---------------------
        bool sprintKeyHeld =
            Input.GetKey(sprintKey) &&
            isGrounded &&
            !isMovementLocked &&
            IsMoving;

        //if there's a stamina system, enforce gating
        if (staminaUI != null)
        {
            //if stamina hits 0, block sprint until we recover enough
            if (!sprintBlocked && !staminaUI.HasStamina())
                sprintBlocked = true;

            if (sprintBlocked)
            {
                //re-enable sprint only after threshold recovery
                if (staminaUI.HasEnoughToSprint(sprintReenableThreshold))
                    sprintBlocked = false;
            }

            bool canSprintNow = sprintKeyHeld && !sprintBlocked && staminaUI.HasStamina();

            //set exposed state
            IsSprinting = canSprintNow;

            //apply speed
            if (canSprintNow)
            {
                currentSpeed *= sprintMultiplier;
                staminaUI.DrainStamina(Time.deltaTime);
            }
            else
            {
                staminaUI.RegenStamina(Time.deltaTime);
            }
        }
        else
        {
            //if no stamina system assigned: behave like the original sprint logic
            IsSprinting = sprintKeyHeld;

            if (sprintKeyHeld)
                currentSpeed *= sprintMultiplier;
        }

        if (!isMovementLocked)
            controller.Move(move * currentSpeed * Time.deltaTime);

        if (!isMovementLocked && Input.GetKeyDown(jumpKey) && isGrounded)
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity * gravityScale);

        playerVelocity.y += gravity * gravityScale * Time.deltaTime;

        controller.Move(playerVelocity * Time.deltaTime);
    }

    public void SetMovementLock(bool state)
    {
        isMovementLocked = state;

        if (state)
        {
            playerVelocity = Vector3.zero;
            IsSprinting = false;
            IsMoving = false;
        }
    }
}
