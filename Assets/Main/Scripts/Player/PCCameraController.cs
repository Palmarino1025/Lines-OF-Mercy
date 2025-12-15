using UnityEngine;

public class PCCameraController : MonoBehaviour
{
    [Header("Mouse Look")]
    public float mouseSensitivity = 2f;
    public bool holdRightMouseToRotate = false;

    private float currentYaw = 0f;
    private float currentPitch = 0f;
    public bool isCameraLookEnabled = true;

    void Start()
    {
        Vector3 startAngles = transform.eulerAngles;
        currentYaw = startAngles.y;
        currentPitch = startAngles.x;

        // Start with camera look enabled (gameplay mode)
        EnableCameraLook(true);
    }

    void Update()
    {
        // If dialogue is active we do not rotate the camera
        if (!isCameraLookEnabled)
        {
            return;
        }

        // Optional: only rotate while RMB is held
        if (holdRightMouseToRotate && !Input.GetMouseButton(1))
        {
            return;
        }

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        currentYaw += mouseX;
        currentPitch -= mouseY;

        currentPitch = Mathf.Clamp(currentPitch, -80f, 80f);

        transform.rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
    }

    // This will be called when a dialogue opens/closes
    public void EnableCameraLook(bool enableLook)
    {
        isCameraLookEnabled = enableLook;

        if (enableLook)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
