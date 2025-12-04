using UnityEngine;

public class NpcInteraction : MonoBehaviour
{
    // player object Tag
    public string playerTagName = "Player";

    // This will be true only while the player is inside the trigger
    private bool isPlayerInsideInteractionRange = false;

    // This method is called when something enters the trigger collider
    private void OnTriggerEnter(Collider otherCollider)
    {
        // Check if whatever entered has the same tag as player
        if (otherCollider.CompareTag(playerTagName))
        {
            isPlayerInsideInteractionRange = true;

            Debug.Log("NpcInteraction: Player entered interaction range.");
        }
    }

    // This method is called when something exits the trigger collider
    private void OnTriggerExit(Collider otherCollider)
    {
        // Check if whatever left has the same tag as our player
        if (otherCollider.CompareTag(playerTagName))
        {
            isPlayerInsideInteractionRange = false;

            Debug.Log("NpcInteraction: Player left interaction range.");
        }
    }
}
