using UnityEngine;
using UnityEngine.UI;
using PixelCrushers.DialogueSystem;

public class TextSpeedSettings : MonoBehaviour
{
    [Header("References")]
    public Slider textSpeedSlider;
    public UnityUITypewriterEffect typewriterEffect;

    private void Start()
    {
        if (typewriterEffect == null)
        {
            typewriterEffect = FindObjectOfType<UnityUITypewriterEffect>();
        }

        LoadFromPlayerData();

        if (textSpeedSlider != null)
        {
            textSpeedSlider.onValueChanged.AddListener(OnTextSpeedChanged);
        }
    }

    public void OnTextSpeedChanged(float value)
    {
        ApplyTextSpeed(value);
        WriteToPlayerData(value);
    }

    private void ApplyTextSpeed(float value)
    {
        if (typewriterEffect != null)
        {
            typewriterEffect.charactersPerSecond = value;
        }
    }

    private void LoadFromPlayerData()
    {
        if (GameManager.Instance == null) return;

        float savedSpeed = GameManager.Instance.playerData.textSpeed;

        if (textSpeedSlider != null)
        {
            textSpeedSlider.SetValueWithoutNotify(savedSpeed);
        }

        ApplyTextSpeed(savedSpeed);
    }

    private void WriteToPlayerData(float value)
    {
        GameManager.Instance.playerData.textSpeed = value;
        GameManager.Instance.SavePlayerData();
    }
}