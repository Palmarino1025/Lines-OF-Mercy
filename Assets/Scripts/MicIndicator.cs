using UnityEngine;
using UnityEngine.UI;

public class MicIndicator : MonoBehaviour
{
    [Header("UI Reference")]
    public Image micImage;

    [Header("Mic Colors")]
    public Color micOnColor = new Color32(110, 114, 99, 255);   // #6E7263
    public Color micOffColor = new Color32(138, 3, 3, 255);     // #8A0303

    [Header("State")]
    public bool isMicOn = false;

    void Start()
    {
        UpdateMicColor();
    }

    public void ToggleMic(bool state)
    {
        isMicOn = state;
        UpdateMicColor();
    }

    private void UpdateMicColor()
    {
        if (micImage != null)
            micImage.color = isMicOn ? micOnColor : micOffColor;
    }
}
