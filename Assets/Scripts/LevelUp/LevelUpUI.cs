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

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnEnable()
    {
        if (LevelManager.Instance != null)
            LevelManager.Instance.OnLevelUp += ShowLevelUpUI;
    }

    private void OnDisable()
    {
        if (LevelManager.Instance != null)
            LevelManager.Instance.OnLevelUp -= ShowLevelUpUI;
    }

    private void ShowLevelUpUI()
    {
        if (topUIPanel != null) topUIPanel.SetActive(false);
        if (levelUpPanel != null) levelUpPanel.SetActive(true);
        Time.timeScale = 0f;

        StatUpgrade[] choices = GetRandomUpgrades(cards.Length);
        for (int i = 0; i < cards.Length; i++)
        {
            if (i < choices.Length)
                cards[i].Setup(choices[i]);
        }
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
        if (topUIPanel != null) topUIPanel.SetActive(true);
        if (levelUpPanel != null) levelUpPanel.SetActive(false);
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