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
    [SerializeField] private TMP_Text statBuffText;
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
    [SerializeField] private float interactableDelay = 0.6f;

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

        if (statBuffText != null)
            statBuffText.text = FormatBuffText(upgrade.statType, upgrade.upgradeAmount);

        if (iconImage != null)
            iconImage.sprite = upgrade.icon;

        if (cardBackgroundImage != null)
            cardBackgroundImage.sprite = GetRarityBackground(upgrade.rarity);

        gameObject.SetActive(false);
    }

    private string FormatBuffText(StatType statType, float amount)
    {
        return statType switch
        {
            StatType.Damage        => $"+{amount} Damage",
            StatType.FireRate      => $"+{amount:F2}s Attack Speed",
            StatType.MoveSpeed     => $"+{amount} Move Speed",
            StatType.MaxHealth     => $"+{amount} Max HP",
            StatType.CritChance    => $"+{amount * 100:F0}% Crit Chance",
            StatType.CritMultiplier => $"+{amount:F1}x Crit Damage",
            StatType.BulletSpeed   => $"+{amount} Bullet Speed",
            _ => $"+{amount}"
        };
    }

    public IEnumerator AnimateIn(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        gameObject.SetActive(true);
        isInteractable = false;

        // Reset CanvasGroup alpha in case it was dismissed before
        CanvasGroup cg = GetComponent<CanvasGroup>();
        if (cg != null) cg.alpha = 1f;

        transform.localScale = Vector3.zero;
        float t = 0f;
        while (t < popInDuration)
        {
            t += Time.unscaledDeltaTime;
            float progress = t / popInDuration;
            transform.localScale = Vector3.one * Mathf.Lerp(0f, 1.1f, Mathf.SmoothStep(0f, 1f, progress));
            yield return null;
        }
        transform.localScale = Vector3.one;

        originalPosition = ((RectTransform)transform).anchoredPosition;
        originalScale = transform.localScale;

        yield return new WaitForSecondsRealtime(interactableDelay);
        isInteractable = true;
    }

    // Called by LevelUpUI when another card is selected
    public void MakeNonInteractable()
    {
        isInteractable = false;
        if (hoverCoroutine != null)
        {
            StopCoroutine(hoverCoroutine);
            hoverCoroutine = null;
        }
        // Snap back to rest position
        ((RectTransform)transform).anchoredPosition = originalPosition;
        transform.localScale = originalScale;
    }

    public IEnumerator PlaySelectAnimation()
    {
        float t = 0f;
        float duration = 0.45f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float progress = t / duration;
            // Punch out then settle
            float punch = 1f + Mathf.Sin(progress * Mathf.PI) * 0.25f;
            transform.localScale = originalScale * punch;
            yield return null;
        }
        transform.localScale = originalScale;
    }

    public IEnumerator PlayDismissAnimation()
    {
        CanvasGroup cg = GetComponent<CanvasGroup>();
        if (cg == null) cg = gameObject.AddComponent<CanvasGroup>();

        float t = 0f;
        float duration = 0.3f;
        Vector3 startScale = transform.localScale;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float progress = Mathf.SmoothStep(0f, 1f, t / duration);
            transform.localScale = Vector3.Lerp(startScale, startScale * 0.8f, progress);
            cg.alpha = Mathf.Lerp(1f, 0f, progress);
            yield return null;
        }

        transform.localScale = startScale * 0.8f;
        cg.alpha = 0f;
    }

    public void OnCardClicked()
    {
        if (!isInteractable || upgrade == null) return;
        if (LevelUpUI.Instance != null)
            LevelUpUI.Instance.SelectUpgrade(upgrade, this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isInteractable) return;
        if (hoverCoroutine != null) StopCoroutine(hoverCoroutine);
        hoverCoroutine = StartCoroutine(AnimateHover(true));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isInteractable) return;
        if (hoverCoroutine != null) StopCoroutine(hoverCoroutine);
        hoverCoroutine = StartCoroutine(AnimateHover(false));
    }

    private IEnumerator AnimateHover(bool isEntering)
    {
        float t = 0f;
        Vector3 startScale = transform.localScale;
        Vector3 startPos = ((RectTransform)transform).anchoredPosition;

        // Always target relative to originalPosition so it always returns correctly
        Vector3 targetScale = isEntering ? originalScale * hoverScale : originalScale;
        Vector3 targetPos  = isEntering ? originalPosition + (Vector3)hoverOffset : originalPosition;

        while (t < hoverDuration)
        {
            t += Time.unscaledDeltaTime;
            float progress = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t / hoverDuration));
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
            UpgradeRarity.Common    => commonBackground,
            UpgradeRarity.Uncommon  => uncommonBackground,
            UpgradeRarity.Rare      => rareBackground,
            UpgradeRarity.Epic      => epicBackground,
            UpgradeRarity.Legendary => legendaryBackground,
            _ => commonBackground
        };
    }
}