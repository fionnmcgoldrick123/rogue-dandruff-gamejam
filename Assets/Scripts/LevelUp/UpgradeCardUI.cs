using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class UpgradeCardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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
    [SerializeField] private float hoverScale = 1.1f;
    [SerializeField] private float hoverDuration = 0.15f;
    [SerializeField] private Vector2 hoverOffset = new Vector2(0, 20f);
    [SerializeField] private float interactableDelay = 1.0f;

    private StatUpgrade upgrade;
    private Vector3 originalPosition;
    private Vector3 originalScale;
    private Coroutine hoverCoroutine;
    private bool isInteractable = false;

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

        isInteractable = false;

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
        
        // Store final position for hover animation
        originalPosition = ((RectTransform)transform).anchoredPosition;
        originalScale = transform.localScale;

        // Wait before allowing interaction
        yield return new WaitForSecondsRealtime(interactableDelay);
        isInteractable = true;
    }

    public void OnCardClicked()
    {
        if (!isInteractable || upgrade == null) return;
        if (LevelUpUI.Instance != null)
            LevelUpUI.Instance.ApplyUpgrade(upgrade);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isInteractable) return;
        
        if (hoverCoroutine != null)
            StopCoroutine(hoverCoroutine);
        hoverCoroutine = StartCoroutine(AnimateHover(true));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isInteractable) return;
        
        if (hoverCoroutine != null)
            StopCoroutine(hoverCoroutine);
        hoverCoroutine = StartCoroutine(AnimateHover(false));
    }

    private IEnumerator AnimateHover(bool isEntering)
    {
        float t = 0f;
        Vector3 startScale = transform.localScale;
        Vector3 startPos = ((RectTransform)transform).anchoredPosition;
        
        Vector3 targetScale = isEntering ? Vector3.one * hoverScale : originalScale;
        Vector3 targetPos = isEntering ? startPos + (Vector3)hoverOffset : originalPosition;

        while (t < hoverDuration)
        {
            t += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(t / hoverDuration);
            progress = Mathf.SmoothStep(0f, 1f, progress);

            transform.localScale = Vector3.Lerp(startScale, targetScale, progress);
            ((RectTransform)transform).anchoredPosition = Vector3.Lerp(startPos, targetPos, progress);
            yield return null;
        }

        transform.localScale = targetScale;
        ((RectTransform)transform).anchoredPosition = targetPos;
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