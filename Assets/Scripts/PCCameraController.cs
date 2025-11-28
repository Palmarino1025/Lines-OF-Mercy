using UnityEngine;

// Basic WASD camera movement on the ground
public class PCCameraController : MonoBehaviour
{
    // How fast the camera moves.
    public float moveSpeed = 15f;
    // Faster movement speed while Left Shift held down
    public float fastMoveSpeed = 25f;
    // Key will activate the faster speed.
    public KeyCode fastMoveKey = KeyCode.LeftShift;


    private void Update()
    {
        HandleMovement();
    }

    // Handle WASD movement.
    private void HandleMovement()
    {
        // Get player input from Unity's Input Manager.
        // Horizontal = A / D or Left / Right arrows.
        float horizontalInput = Input.GetAxis("Horizontal");

        // Vertical = W / S or Up / Down arrows.
        float verticalInput = Input.GetAxis("Vertical");

        // Forward direction but flattened on the XZ plane (no vertical movement).
        Vector3 flatForwardDirection = new Vector3(
            transform.forward.x,
            0f,
            transform.forward.z
        );

        // Right direction but flattened on the XZ plane.
        Vector3 flatRightDirection = new Vector3(
            transform.right.x,
            0f,
            transform.right.z
        );

        // Normalize so directions have length 1.
        flatForwardDirection = flatForwardDirection.normalized;
        flatRightDirection = flatRightDirection.normalized;

        // Combine directions based on input.
        Vector3 moveDirection =
            flatForwardDirection * verticalInput +
            flatRightDirection * horizontalInput;

        // Prevent faster diagonal movement.
        if (moveDirection.sqrMagnitude > 1f)
        {
            moveDirection = moveDirection.normalized;
        }

        // Choose which speed to use this frame.
        float moveSpeedThisFrame = moveSpeed;

        // If the fast move key is held, use the faster speed.
        if (Input.GetKey(fastMoveKey))
        {
            moveSpeedThisFrame = fastMoveSpeed;
        }

        // Move the camera.
        transform.position += moveDirection * moveSpeedThisFrame * Time.deltaTime;
    }
}
