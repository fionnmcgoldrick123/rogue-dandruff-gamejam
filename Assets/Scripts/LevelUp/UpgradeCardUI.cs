using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeCardUI : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text rarityText;
    [SerializeField] private Image iconImage;
    [SerializeField] private Image cardBackgroundImage;

    [Header("Rarity Card Backgrounds")]
    [SerializeField] private Sprite commonBackground;
    [SerializeField] private Sprite uncommonBackground;
    [SerializeField] private Sprite rareBackground;
    [SerializeField] private Sprite epicBackground;
    [SerializeField] private Sprite legendaryBackground;

    [Header("Animation")]
    [SerializeField] private float popInDuration = 0.25f;

    private StatUpgrade upgrade;

    public void Setup(StatUpgrade upgrade)
    {
        this.upgrade = upgrade;

        nameText.text = upgrade.upgradeName;
        descriptionText.text = upgrade.description;
        rarityText.text = upgrade.rarity.ToString();

        if (iconImage != null)
            iconImage.sprite = upgrade.icon;

        if (cardBackgroundImage != null)
            cardBackgroundImage.sprite = GetRarityBackground(upgrade.rarity);

        gameObject.SetActive(false);
    }

    public IEnumerator AnimateIn(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        gameObject.SetActive(true);

        transform.localScale = Vector3.zero;
        float t = 0f;
        while (t < popInDuration)
        {
            t += Time.unscaledDeltaTime;
            float progress = t / popInDuration;
            // Overshoot spring feel
            float scale = Mathf.Sin(progress * Mathf.PI * 0.5f);
            scale = 1f + (scale - 1f) * Mathf.Cos(progress * Mathf.PI);
            transform.localScale = Vector3.one * Mathf.Lerp(0f, 1.1f, Mathf.SmoothStep(0f, 1f, progress));
            yield return null;
        }
        transform.localScale = Vector3.one;
    }

    public void OnCardClicked()
    {
        if (upgrade == null) return;
        if (LevelUpUI.Instance != null)
            LevelUpUI.Instance.ApplyUpgrade(upgrade);
    }

    private Sprite GetRarityBackground(UpgradeRarity rarity)
    {
        return rarity switch
        {
            UpgradeRarity.Common => commonBackground,
            UpgradeRarity.Uncommon => uncommonBackground,
            UpgradeRarity.Rare => rareBackground,
            UpgradeRarity.Epic => epicBackground,
            UpgradeRarity.Legendary => legendaryBackground,
            _ => commonBackground
        };
    }
}