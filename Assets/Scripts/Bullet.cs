using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 direction;
    private float timer;
    private ObjectPool pool;

    private float damage;
    private bool isCrit;
    private float speed;
    private float lifespan;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Init(ObjectPool pool, Vector2 direction, PlayerStats stats)
    {
        this.pool = pool;
        this.direction = direction.normalized;

        speed = stats.bulletSpeed;
        lifespan = stats.bulletLifespan;
        timer = lifespan;

        isCrit = Random.value < stats.critChance;
        damage = isCrit ? stats.damage * stats.critMultiplier : stats.damage;

        float angle = Mathf.Atan2(this.direction.y, this.direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
            ReturnToPool();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = direction * speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Vector3 hitPosition = other.ClosestPoint(transform.position);

        if (other.TryGetComponent(out Enemy enemy))
        {
            enemy.TakeDamage(damage, direction);
            SpawnDamageNumber(hitPosition);
            ReturnToPool();
        }
        else if (other.TryGetComponent(out Hair hair))
        {
            hair.TakeDamage(damage);
            SpawnDamageNumber(hitPosition);
            ReturnToPool();
        }
        else if (other.TryGetComponent(out IHittable hittable))
        {
            hittable.OnHit();
            SpawnDamageNumber(hitPosition);
            ReturnToPool();
        }
    }

    private void SpawnDamageNumber(Vector3 position)
    {
        if (DamageNumberManager.Instance != null)
            DamageNumberManager.Instance.SpawnDamageNumber(position, damage, isCrit);
    }

    private void ReturnToPool()
    {
        rb.linearVelocity = Vector2.zero;
        pool.Return(gameObject);
    }
}