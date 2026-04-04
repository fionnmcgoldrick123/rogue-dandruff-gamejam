using UnityEngine;

public class Hair : MonoBehaviour, IHittable
{
    [Header("Hair Settings")]
    [SerializeField] private float cutTime;

    [Header("Health Settings")]
    [SerializeField] private float health = 3f;
    [SerializeField] private int coinCount = 5;
    [SerializeField] private ObjectPool coinPool;

    private float maxHealth;
    private float chargeTime;
    public bool playerInside { get; set; }
    private HitFlash hitFlash;

    private void Awake()
    {
        hitFlash = GetComponent<HitFlash>();
        maxHealth = health;
    }

    private void Update()
    {   

        if (playerInside)
            Cutting();
        else
            NotCutting();

    }

    private void Cutting()
    {
        Debug.Log("Cutting hair... Charge Time: " + chargeTime);
        chargeTime += Time.deltaTime;

        if (chargeTime >= cutTime)
        {
            Debug.Log("Hair Cut!");
            chargeTime = 0f;
        }
    }

    private void NotCutting()
    {
        Debug.Log("Not cutting hair... Charge Time: " + chargeTime);
        chargeTime -= Time.deltaTime;
        if (chargeTime < 0f)
            chargeTime = 0f;
    }

    public void OnHit()
    {
        // Trigger hit flash effect via IHittable (for non-bullet hits)
        if (hitFlash != null)
            hitFlash.OnHit();
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        // Trigger visual feedback
        if (hitFlash != null)
            hitFlash.OnHit();

        if (health <= 0f)
            Die();
    }

    private void Die()
    {
        SpawnCoins();
        gameObject.SetActive(false);
    }

    private void SpawnCoins()
    {
        if (coinPool == null)
        {
            Debug.LogWarning("Coin pool not assigned to Hair!");
            return;
        }

        for (int i = 0; i < coinCount; i++)
        {
            // Spawn coins in a circle around the hair's center
            float angle = (360f / coinCount) * i;
            Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

            GameObject coinObj = coinPool.Get(transform.position, Quaternion.identity);
            Coin coin = coinObj.GetComponent<Coin>();
            coin.Launch(direction, coinPool);
        }
    }
}
