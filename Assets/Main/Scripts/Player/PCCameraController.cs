using UnityEngine;

public class PCCameraController : MonoBehaviour
{
    [Header("Mouse Look")]
    public float mouseSensitivity = 2f;
    public bool holdRightMouseToRotate = false;

    //Reference to the player root object so left/right rotation happens on the player, not just the camera
    public Transform playerTransform;

    private float currentYaw = 0f;
    private float currentPitch = 0f;
    public bool isCameraLookEnabled = false;

    void Start()
    {
        //Use the player rotation for yaw if a player transform was assigned
        Vector3 startAngles = playerTransform != null ? playerTransform.eulerAngles : transform.eulerAngles;
        currentYaw = startAngles.y;
        currentPitch = transform.eulerAngles.x;

        // Start with camera look enabled (gameplay mode)
        EnableCameraLook(false);
    }

    void Update()
    {
        // If dialogue is active we do not rotate the camera
        if (!isCameraLookEnabled)
        {
            return;
        }

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        currentYaw += mouseX;
        currentPitch -= mouseY;

        currentPitch = Mathf.Clamp(currentPitch, -80f, 80f);

        //Rotate the player left/right on the Y axis
        if (playerTransform != null)
        {
            //Apply horizontal turning to the player object
            playerTransform.rotation = Quaternion.Euler(0f, currentYaw, 0f);
        }

        //Rotate only the camera up/down on the X axis
        transform.localRotation = Quaternion.Euler(currentPitch, 0f, 0f);
    }

    // This will be called when a dialogue opens/closes
    public void EnableCameraLook(bool enableLook)
    {
        isCameraLookEnabled = enableLook;

        if (!enableLook)
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