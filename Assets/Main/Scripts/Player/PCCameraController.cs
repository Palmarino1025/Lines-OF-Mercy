using UnityEngine;

public class PCCameraController : MonoBehaviour
{
    public float mouseSensitivity = 2f;   // Mouse sensitivity multiplier
    public bool holdRightMouseToRotate = false; // If true, rotate only when RMB held

    private float currentYaw = 0f;   // Horizontal rotation (Y axis)
    private float currentPitch = 0f; // Vertical rotation (X axis)

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        currentYaw = angles.y;
        currentPitch = angles.x;

        // Optional: lock cursor to center and hide it
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (holdRightMouseToRotate && !Input.GetMouseButton(1))
            return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        currentYaw += mouseX;
        currentPitch -= mouseY;

        currentPitch = Mathf.Clamp(currentPitch, -80f, 80f);

        transform.rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
    }
}
