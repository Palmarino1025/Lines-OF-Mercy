using UnityEngine;


[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float sprintMultiplier = 1.7f;     // how fast sprinting is relative to walk
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

    public bool IsSprinting {  get; private set; }
    public bool IsMoving { get; private set; }

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (cameraTransform == null)
            Debug.LogError("Assign the Camera Transform to PlayerMovement.");
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

        // ---------------------
        //   🏃 Sprint Logic
        // ---------------------
        float currentSpeed = moveSpeed;

        IsMoving = move.magnitude > 0.1f;

        IsSprinting =
            Input.GetKey(sprintKey) &&
            isGrounded &&
            !isMovementLocked &&
            IsMoving;

        if (Input.GetKey(sprintKey) && isGrounded && !isMovementLocked && move.magnitude > 0.1f)
            currentSpeed *= sprintMultiplier;

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

        // Also stop current motion so they don't slide while frozen
        if (state)
        {
            playerVelocity = Vector3.zero;
            IsSprinting = false;
            IsMoving = false;
        }
        
    }
}
