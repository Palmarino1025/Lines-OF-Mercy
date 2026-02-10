using UnityEngine;

public class HUDVisibilityController : MonoBehaviour
{
    public CanvasGroup hudCanvasGroup;

    private void Awake()
    {
        if (hudCanvasGroup == null)
        {
            hudCanvasGroup = GetComponent<CanvasGroup>();
        }
    }

    public void HideHUD()
    {
        if (hudCanvasGroup == null) return;

        hudCanvasGroup.alpha = 0f;
        hudCanvasGroup.interactable = false;
        hudCanvasGroup.blocksRaycasts = false;
    }

    public void ShowHUD()
    {
        if (hudCanvasGroup == null) return;

        hudCanvasGroup.alpha = 1f;
        hudCanvasGroup.interactable = true;
        hudCanvasGroup.blocksRaycasts = true;
    }
}
