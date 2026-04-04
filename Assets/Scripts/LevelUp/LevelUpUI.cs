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

    private Camera mainCamera;

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

        // Subscribe here after all Awake calls have run
        if (LevelManager.Instance != null)
            LevelManager.Instance.OnLevelUp += OnLevelUp;
    }

    private void OnDestroy()
    {
        if (LevelManager.Instance != null)
            LevelManager.Instance.OnLevelUp -= OnLevelUp;
    }

    private void OnLevelUp()
    {
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
        // Let popup play briefly before pausing
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

        // Animate cards in one at a time
        for (int i = 0; i < cards.Length; i++)
            StartCoroutine(cards[i].AnimateIn(i * delayBetweenCards));
    }

    public void ApplyUpgrade(StatUpgrade upgrade)
    {
        if (playerStats == null || upgrade == null) return;

        switch (upgrade.statType)
        {
            case StatType.Damage:         playerStats.damage         += upgrade.upgradeAmount; break;
            case StatType.FireRate:       playerStats.fireRate        = Mathf.Max(0.05f, playerStats.fireRate - upgrade.upgradeAmount); break;
            case StatType.MoveSpeed:      playerStats.moveSpeed      += upgrade.upgradeAmount; break;
            case StatType.MaxHealth:      playerStats.maxHealth      += upgrade.upgradeAmount; break;
            case StatType.CritChance:     playerStats.critChance      = Mathf.Clamp01(playerStats.critChance + upgrade.upgradeAmount); break;
            case StatType.CritMultiplier: playerStats.critMultiplier += upgrade.upgradeAmount; break;
            case StatType.BulletSpeed:    playerStats.bulletSpeed    += upgrade.upgradeAmount; break;
        }

        HideLevelUpUI();
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