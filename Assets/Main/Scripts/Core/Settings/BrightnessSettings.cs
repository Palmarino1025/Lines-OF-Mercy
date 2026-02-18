using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Only loads after SettingsCanvas is loaded.Needs to be fixed.

public class BrightnessSettings : MonoBehaviour
{
    [Header("References")]
    public Image brightnessOverlay;
    public Slider brightnessSlider;

    private void Start()
    {
        if (brightnessSlider != null)
        {
            brightnessSlider.onValueChanged.AddListener(OnBrightnessChanged);
        }

        LoadFromPlayerData();
    }

    public void OnBrightnessChanged(float value)
    {
        ApplyBrightness(value);
        WriteToPlayerData(value);
    }

    private void ApplyBrightness(float value)
    {
        if (brightnessOverlay == null) return;

        float clamped = Mathf.Clamp(value, 0.3f, 1f);

        Color color = brightnessOverlay.color;
        color.a = 1f - clamped;
        brightnessOverlay.color = color;
    }
    
    private void LoadFromPlayerData()
    {
        if (DataManager.Instance == null) return;

        float savedBrightness = DataManager.Instance.playerData.brightness;

        if (brightnessSlider != null)
        {
            brightnessSlider.SetValueWithoutNotify(savedBrightness);
        }

        ApplyBrightness(savedBrightness);
    }

    private void WriteToPlayerData(float value)
    {
        DataManager.Instance.playerData.brightness = value;
        DataManager.Instance.SavePlayerData();
    }
}