using UnityEngine;
using UnityEngine.UI;

public class XPBarUI : MonoBehaviour
{
    [SerializeField] private Image fillImage;

    private void Start()
    {
        // Auto-find if not assigned
        if (fillImage == null)
            fillImage = GetComponent<Image>();

        if (fillImage == null)
            Debug.LogError("XPBarUI: No Image component found! Assign it in the Inspector or attach to the Image GameObject.");
    }

    private void Update()
    {
        if (LevelManager.Instance == null || fillImage == null)
            return;

        float progress = LevelManager.Instance.GetExpProgress();
        fillImage.fillAmount = progress;
    }
}