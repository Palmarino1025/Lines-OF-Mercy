using UnityEngine;

public class MinimapFollow : MonoBehaviour
{
  
    public Transform playerTransform;
    public float minimapHeight = 30f;

    void LateUpdate()
    {
        // Stop the script if the player was not assigned
        if (playerTransform == null)
        {
            return;
        }

        // Copy the player's position
        Vector3 newCameraPosition = playerTransform.position;

        // camera height so it stays above the player
        newCameraPosition.y = minimapHeight;

        // Move the minimap camera to the new position
        transform.position = newCameraPosition;

        // Make the minimap rotate with the player
        // X = 90 degrees keeps the camera looking straight down
        // Y = player rotation so the map rotates when the player turns
        transform.rotation = Quaternion.Euler(
            90f,
            playerTransform.eulerAngles.y,
            0f
        );
    }
}