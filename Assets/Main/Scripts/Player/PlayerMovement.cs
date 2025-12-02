using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 6f;
    public Transform cameraTransform;  // Assign your main camera here in Inspector

    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();

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

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        // Calculate movement direction
        Vector3 move = right * x + forward * z;

        if (move.magnitude > 1f)
            move.Normalize();

        // Move the player
        controller.Move(move * moveSpeed * Time.deltaTime);
    }
}
