using UnityEngine;
using UnityEngine.UI;

public class StaminaUI : MonoBehaviour
{
    [Header("UI")]
    public Image staminaFill;

    [Header("Stamina")]
    public float maxStamina = 100f;
    public float currentStamina;

    [Header("Rates")]
    public float drainRate = 25f;     // per second while sprinting
    public float regenRate = 15f;     // per second while not sprinting

    void Start()
    {
        currentStamina = maxStamina;
        UpdateUI();
    }

    public void DrainStamina(float deltaTime)
    {
        currentStamina -= drainRate * deltaTime;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        UpdateUI();
    }

    public void RegenStamina(float deltaTime)
    {
        currentStamina += regenRate * deltaTime;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        UpdateUI();
    }

    public bool HasStamina()
    {
        return currentStamina > 0;
    }

    void UpdateUI()
    {
        staminaFill.fillAmount = currentStamina / maxStamina;
    }
}
