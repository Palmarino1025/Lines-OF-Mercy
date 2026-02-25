using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using PixelCrushers.DialogueSystem;

public class DialogueTextSpeedApplier : MonoBehaviour
{
    private void OnEnable()
    {
        DialogueManager.instance.conversationStarted += ApplyTextSpeed;
    }

    private void OnDisable()
    {
        if (DialogueManager.instance != null)
        {
            DialogueManager.instance.conversationStarted -= ApplyTextSpeed;
        }
    }

    public void ApplyTextSpeed(Transform actor)
    {
        float speed = DataManager.Instance.GetTextSpeed();

        UnityUITypewriterEffect typewriter = FindObjectOfType<UnityUITypewriterEffect>();

        if (typewriter != null)
        {
            typewriter.charactersPerSecond = speed;
            UnityEngine.Debug.Log("[Dialogue] Text speed applied: " + speed);
        }
    }
}