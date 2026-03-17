using UnityEngine;
using UnityEngine.UI;

public class StaminaUI : MonoBehaviour
{
    [Header("UI")]
    public Image staminaFill;

    [Header("Bar Colors")]
    public Color normalStaminaColor = new Color(0.80f, 0.72f, 0.50f, 1f);
    public Color lowStaminaColor = new Color(0.85f, 0.20f, 0.20f, 1f);

    [Header("Stamina")]
    public float maxStamina = 100f;
    public float currentStamina;

    [Header("Rates")]
    public float drainRate = 18f;          // stamina lost per second while sprinting
    public float regenRate = 24f;          // stamina gained per second while recovering
    public float regenDelayAfterSprint = 1.1f; // delay before stamina starts regenerating

    [Header("UI Feel")]
    public float uiFillSmoothSpeed = 8f;   // higher = faster visual response

    [Range(0f, 1f)]
    public float lowStaminaThreshold = 0.25f;

    // This is the value the player sees on screen.
    // It smoothly chases the real stamina amount.
    private float displayedNormalizedStamina = 1f;

    // Timer that prevents stamina from instantly regenerating
    private float regenDelayTimer = 0f;

    void Start()
    {
        currentStamina = maxStamina;
        displayedNormalizedStamina = 1f;
        UpdateUIInstant();
    }

    void Update()
    {
        // Count down the delay before stamina can regenerate
        if (regenDelayTimer > 0f)
        {
            regenDelayTimer -= Time.deltaTime;
        }

        SmoothUpdateUI();
    }

    public void DrainStamina(float deltaTime)
    {
        currentStamina -= drainRate * deltaTime;
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);

        // Every time the player sprints, restart the regen delay
        regenDelayTimer = regenDelayAfterSprint;
    }

    public void RegenStamina(float deltaTime)
    {
        // Do not regenerate until the delay is finished
        if (regenDelayTimer > 0f)
        {
            return;
        }

        currentStamina += regenRate * deltaTime;
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
    }

    public bool HasStamina()
    {
        return currentStamina > 0f;
    }

    public float Normalized()
    {
        if (maxStamina <= 0f)
        {
            return 0f;
        }

        return currentStamina / maxStamina;
    }

    public bool HasEnoughToSprint(float requiredPercent)
    {
        return Normalized() >= requiredPercent;
    }

    void SmoothUpdateUI()
    {
        float targetNormalizedStamina = Normalized();

        displayedNormalizedStamina = Mathf.Lerp(
            displayedNormalizedStamina,
            targetNormalizedStamina,
            uiFillSmoothSpeed * Time.deltaTime
        );

        // Snap when very close so it does not jitter forever
        if (Mathf.Abs(displayedNormalizedStamina - targetNormalizedStamina) < 0.001f)
        {
            displayedNormalizedStamina = targetNormalizedStamina;
        }

        if (staminaFill != null)
        {
            staminaFill.fillAmount = displayedNormalizedStamina;

            // Change color when stamina is low
            if (targetNormalizedStamina <= lowStaminaThreshold)
            {
                float lowStaminaBlend = 0f;

                if (lowStaminaThreshold > 0f)
                {
                    lowStaminaBlend = targetNormalizedStamina / lowStaminaThreshold;
                }

                staminaFill.color = Color.Lerp(lowStaminaColor, normalStaminaColor, lowStaminaBlend);
            }
            else
            {
                staminaFill.color = normalStaminaColor;
            }
        }
    }

    void UpdateUIInstant()
    {
        if (staminaFill != null)
        {
            staminaFill.fillAmount = Normalized();
            staminaFill.color = normalStaminaColor;
        }
    }
}