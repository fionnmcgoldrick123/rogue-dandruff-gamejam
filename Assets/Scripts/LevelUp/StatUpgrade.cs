using UnityEngine;

public enum StatType
{
    Damage,
    FireRate,
    MoveSpeed,
    MaxHealth,
    CritChance,
    CritMultiplier,
    BulletSpeed
}

public enum UpgradeRarity { Common, Uncommon, Rare, Epic, Legendary }

[CreateAssetMenu(fileName = "StatUpgrade", menuName = "Rogue Dandruff/Stat Upgrade")]
public class StatUpgrade : ScriptableObject
{
    public string upgradeName;
    [TextArea] public string description;
    public StatType statType;
    public float upgradeAmount;
    public Sprite icon;
    public UpgradeRarity rarity = UpgradeRarity.Common;
}