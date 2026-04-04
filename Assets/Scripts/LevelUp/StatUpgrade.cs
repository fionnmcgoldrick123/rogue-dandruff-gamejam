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

[CreateAssetMenu(fileName = "StatUpgrade", menuName = "Rogue Dandruff/Stat Upgrade")]
public class StatUpgrade : ScriptableObject
{
    public string upgradeName;
    [TextArea] public string description;
    public StatType statType;
    public float upgradeAmount;
    public Sprite icon;
}