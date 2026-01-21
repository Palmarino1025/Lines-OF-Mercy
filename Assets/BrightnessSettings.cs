using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BrightnessSettings : MonoBehaviour
{
    [Header("References")]
    public Image brightnessOverlay;
    public Slider brightnessSlider;

    private void Start()
    {
        if (brightnessSlider != null)
        {
            brightnessSlider.onValueChanged.AddListener(SetBrightness);
            SetBrightness(brightnessSlider.value);
        }
    }

    public void SetBrightness(float value)
    {
        if (brightnessOverlay == null) return;

        // Clamp brightness so it never goes below 0.3
        float clampedBrightness = Mathf.Clamp(value, 0.3f, 1f);

        // Inverted mapping:
        // 0.3 → dark (but visible)
        // 1.0 → fully bright
        Color color = brightnessOverlay.color;
        color.a = 1f - clampedBrightness;
        brightnessOverlay.color = color;
    }
}