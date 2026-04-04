using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour, IHittable
{
    [Header("Enemy Settings")]
    public float moveSpeed = 2f;
    public float health = 3f;
    [SerializeField] private int expOrbCount = 5;

    private float maxHealth;
    private Transform player;
    private ObjectPool enemyPool;
    private ObjectPool expOrbPool;
    private HitFlash hitFlash;

    private void Awake()
    {
        hitFlash = GetComponent<HitFlash>();
        maxHealth = health;
    }

    private void OnEnable()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    public void Init(ObjectPool enemyPool, ObjectPool expOrbPool)
    {
        this.enemyPool = enemyPool;
        this.expOrbPool = expOrbPool;
        health = maxHealth;
    }

    private void Update()
    {
        MoveTowardsPlayer();
    }

    private void MoveTowardsPlayer()
    {
        if (player == null) return;
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    public void TakeDamage(float damage)
    {
        TakeDamage(damage, Vector2.zero);
    }

    public void OnHit()
    {
        // Trigger hit flash effect via IHittable
        if (hitFlash != null)
            hitFlash.OnHit();
    }

    public void TakeDamage(float damage, Vector2 hitDirection)
    {
        health -= damage;
        OnHit();
        if (health <= 0f)
            Die(hitDirection);
    }

    private void Die(Vector2 hitDirection)
    {
        SpawnExpOrbs(hitDirection);
        enemyPool.Return(gameObject);
    }

    private void SpawnExpOrbs(Vector2 hitDirection)
    {
        for (int i = 0; i < expOrbCount; i++)
        {
            GameObject orbObj = expOrbPool.Get(transform.position, Quaternion.identity);
            ExpOrb orb = orbObj.GetComponent<ExpOrb>();

            Vector2 dir;
            if (hitDirection == Vector2.zero)
            {
                dir = Random.insideUnitCircle.normalized;
            }
            else
            {
                float spread = Random.Range(-90f, 90f);
                dir = Quaternion.Euler(0, 0, spread) * (-hitDirection.normalized);
            }

            orb.Launch(dir, expOrbPool);
        }
    }
}
