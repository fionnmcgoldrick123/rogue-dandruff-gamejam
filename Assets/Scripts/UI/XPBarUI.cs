using UnityEngine;
using UnityEngine.UI;

public class XPBarUI : MonoBehaviour
{
    [SerializeField] private Image fillImage;

    private void Update()
    {
        if (LevelManager.Instance == null || fillImage == null) return;
        fillImage.fillAmount = LevelManager.Instance.GetExpProgress();
    }
}