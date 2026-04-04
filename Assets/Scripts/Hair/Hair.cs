using UnityEngine;

public class Hair : MonoBehaviour, IHittable
{
    [Header("Hair Settings")]
    [SerializeField] private float cutTime;

    [Header("Health Settings")]
    [SerializeField] private float health = 3f;
    [SerializeField] private int expOrbCount = 5;
    [SerializeField] private ObjectPool expOrbPool;

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
        SpawnExpOrbs();
        gameObject.SetActive(false);
    }

    private void SpawnExpOrbs()
    {
        if (expOrbPool == null)
        {
            Debug.LogWarning("ExpOrb pool not assigned to Hair!");
            return;
        }

        for (int i = 0; i < expOrbCount; i++)
        {
            float angle = (360f / expOrbCount) * i;
            Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

            GameObject orbObj = expOrbPool.Get(transform.position, Quaternion.identity);
            ExpOrb orb = orbObj.GetComponent<ExpOrb>();
            orb.Launch(direction, expOrbPool);
        }
    }
}
