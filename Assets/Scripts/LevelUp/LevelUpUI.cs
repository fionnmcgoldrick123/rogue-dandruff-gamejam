using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpUI : MonoBehaviour
{
    public static LevelUpUI Instance { get; private set; }

    [Header("References")]
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private StatUpgrade[] allUpgrades;

    [Header("UI Panels")]
    [SerializeField] private GameObject levelUpPanel;
    [SerializeField] private GameObject topUIPanel;

    [Header("Cards")]
    [SerializeField] private UpgradeCardUI[] cards;
    [SerializeField] private float delayBetweenCards = 0.2f;

    [Header("Level Up Popup")]
    [SerializeField] private GameObject levelUpPopupPrefab;
    [SerializeField] private Transform player;
    [SerializeField] private Canvas rootCanvas;

    [Header("Select Transition")]
    [SerializeField] private float resumeDelay = 1f;

    private Camera mainCamera;
    private bool skipCardsAnimation = false;
    private bool isAnimatingCards = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        mainCamera = Camera.main;
    }

    private void Start()
    {
        if (levelUpPanel != null) levelUpPanel.SetActive(false);

        if (LevelManager.Instance != null)
            LevelManager.Instance.OnLevelUp += OnLevelUp;
    }

    private void OnDestroy()
    {
        if (LevelManager.Instance != null)
            LevelManager.Instance.OnLevelUp -= OnLevelUp;
    }

    private void Update()
    {
        // Allow player to skip card animations ONLY while cards are animating in
        if (isAnimatingCards && !skipCardsAnimation)
        {
            if (Input.anyKeyDown || Input.GetMouseButtonDown(0))
                skipCardsAnimation = true;
        }
    }

    private void OnLevelUp()
    {
        skipCardsAnimation = false;
        SpawnLevelUpPopup();
        StartCoroutine(ShowLevelUpSequence());
    }

    private void SpawnLevelUpPopup()
    {
        if (levelUpPopupPrefab == null || player == null) return;

        Transform canvasParent = rootCanvas != null ? rootCanvas.transform : transform;
        GameObject popup = Instantiate(levelUpPopupPrefab, canvasParent);
        LevelUpPopupText popupText = popup.GetComponent<LevelUpPopupText>();
        if (popupText != null)
            popupText.Init(player.position, mainCamera);
    }

    private IEnumerator ShowLevelUpSequence()
    {
        yield return new WaitForSecondsRealtime(0.5f);

        Time.timeScale = 0f;
        if (GameStateManager.Instance != null)
            GameStateManager.Instance.EnterLevelUp();

        if (topUIPanel != null) topUIPanel.SetActive(false);
        if (levelUpPanel != null) levelUpPanel.SetActive(true);

        StatUpgrade[] choices = GetRandomUpgrades(cards.Length);
        for (int i = 0; i < cards.Length; i++)
        {
            if (i < choices.Length)
                cards[i].Setup(choices[i]);
        }

        isAnimatingCards = true;

        for (int i = 0; i < cards.Length; i++)
        {
            if (skipCardsAnimation)
            {
                for (int j = i; j < cards.Length; j++)
                    StartCoroutine(cards[j].AnimateIn(0f));
                break;
            }
            StartCoroutine(cards[i].AnimateIn(i * delayBetweenCards));
            yield return new WaitForSecondsRealtime(delayBetweenCards);
        }

        isAnimatingCards = false;
    }

    // Called by UpgradeCardUI when a card is clicked
    public void SelectUpgrade(StatUpgrade upgrade, UpgradeCardUI selectedCard)
    {
        // Lock all cards immediately to prevent double-clicks
        foreach (var card in cards)
            card.MakeNonInteractable();

        isAnimatingCards = false;
        StartCoroutine(SelectSequence(upgrade, selectedCard));
    }

    private IEnumerator SelectSequence(StatUpgrade upgrade, UpgradeCardUI selectedCard)
    {
        // Play select on picked card, dismiss the rest simultaneously
        StartCoroutine(selectedCard.PlaySelectAnimation());
        foreach (var card in cards)
        {
            if (card != selectedCard && card.gameObject.activeSelf)
                StartCoroutine(card.PlayDismissAnimation());
        }

        yield return new WaitForSecondsRealtime(resumeDelay);

        ApplyStatUpgrade(upgrade);
        HideLevelUpUI();
    }

    private void ApplyStatUpgrade(StatUpgrade upgrade)
    {
        if (playerStats == null || upgrade == null) return;

        switch (upgrade.statType)
        {
            case StatType.Damage:
                playerStats.damage += upgrade.upgradeAmount;
                break;
            case StatType.FireRate:
                playerStats.fireRate = Mathf.Max(0.05f, playerStats.fireRate - upgrade.upgradeAmount);
                break;
            case StatType.MoveSpeed:
                playerStats.moveSpeed += upgrade.upgradeAmount;
                break;
            case StatType.MaxHealth:
                playerStats.maxHealth += upgrade.upgradeAmount;
                break;
            case StatType.CritChance:
                playerStats.critChance = Mathf.Clamp01(playerStats.critChance + upgrade.upgradeAmount);
                break;
            case StatType.CritMultiplier:
                playerStats.critMultiplier += upgrade.upgradeAmount;
                break;
            case StatType.BulletSpeed:
                playerStats.bulletSpeed += upgrade.upgradeAmount;
                break;
        }
    }

    private void HideLevelUpUI()
    {
        foreach (var card in cards)
            card.gameObject.SetActive(false);

        if (levelUpPanel != null) levelUpPanel.SetActive(false);
        if (topUIPanel != null) topUIPanel.SetActive(true);

        Time.timeScale = 1f;
        if (GameStateManager.Instance != null)
            GameStateManager.Instance.EnterCombat();
    }

    private StatUpgrade[] GetRandomUpgrades(int count)
    {
        if (allUpgrades == null || allUpgrades.Length == 0) return new StatUpgrade[0];

        List<StatUpgrade> pool = new List<StatUpgrade>(allUpgrades);
        List<StatUpgrade> result = new List<StatUpgrade>();

        for (int i = 0; i < count && pool.Count > 0; i++)
        {
            int idx = Random.Range(0, pool.Count);
            result.Add(pool[idx]);
            pool.RemoveAt(idx);
        }

        return result.ToArray();
    }
}