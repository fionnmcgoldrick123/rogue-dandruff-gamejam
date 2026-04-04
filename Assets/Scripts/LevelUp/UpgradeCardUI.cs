using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UpgradeCardUI : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Image iconImage;

    private StatUpgrade upgrade;

    public void Setup(StatUpgrade upgrade)
    {
        this.upgrade = upgrade;
        nameText.text = upgrade.upgradeName;
        descriptionText.text = upgrade.description;
        if (iconImage != null && upgrade.icon != null)
            iconImage.sprite = upgrade.icon;
    }

    public void OnCardClicked()
    {
        if (upgrade == null) return;
        if (LevelUpUI.Instance != null)
            LevelUpUI.Instance.ApplyUpgrade(upgrade);
    }
}