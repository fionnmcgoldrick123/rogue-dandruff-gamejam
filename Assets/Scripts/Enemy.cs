using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    public float moveSpeed = 2f;
    public float health = 3f;
    [SerializeField] private int coinCount = 5;

    [Header("Flash Settings")]
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField] private float flashDuration = 0.1f;

    private float maxHealth;
    private Transform player;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private ObjectPool enemyPool;
    private ObjectPool coinPool;
    private Coroutine flashRoutine;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
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

    public void Init(ObjectPool enemyPool, ObjectPool coinPool)
    {
        this.enemyPool = enemyPool;
        this.coinPool = coinPool;
        health = maxHealth;
        flashRoutine = null;
        spriteRenderer.color = originalColor;
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

    public void TakeDamage(float damage, Vector2 hitDirection)
    {
        health -= damage;
        Flash();
        if (health <= 0f)
            Die(hitDirection);
    }

    private void Flash()
    {
        if (flashRoutine != null)
            StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        spriteRenderer.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
        flashRoutine = null;
    }

    private void Die(Vector2 hitDirection)
    {
        SpawnCoins(hitDirection);
        if (flashRoutine != null)
            StopCoroutine(flashRoutine);
        spriteRenderer.color = originalColor;
        enemyPool.Return(gameObject);
    }

    private void SpawnCoins(Vector2 hitDirection)
    {
        for (int i = 0; i < coinCount; i++)
        {
            GameObject coinObj = coinPool.Get(transform.position, Quaternion.identity);
            Coin coin = coinObj.GetComponent<Coin>();

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

            coin.Launch(dir, coinPool);
        }
    }
}
