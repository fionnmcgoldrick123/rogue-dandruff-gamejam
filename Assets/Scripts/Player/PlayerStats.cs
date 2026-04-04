using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "Rogue Dandruff/Player Stats")]
public class PlayerStats : ScriptableObject
{
    [Header("Health")]
    public float maxHealth = 10f;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float acceleration = 50f;
    public float deceleration = 50f;

    [Header("Combat")]
    public float damage = 1f;
    public float fireRate = 0.5f;
    public float bulletSpeed = 15f;
    public float bulletLifespan = 3f;

    [Header("Critical Hits")]
    [Range(0f, 1f)] public float critChance = 0.1f;
    public float critMultiplier = 2f;

    [Header("Damage Number Colors")]
    public Color normalDamageColor = Color.white;
    public Color critColor = Color.yellow;
}