using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 15f;
    [SerializeField] private float lifespan = 3f;
    [SerializeField] private int damage = 1;

    private Rigidbody2D rb;
    private Vector2 direction;
    private float timer;
    private ObjectPool pool;
    private HitMarkerManager hitMarkerManager;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        hitMarkerManager = FindFirstObjectByType<HitMarkerManager>();
    }

    public void Init(ObjectPool pool, Vector2 direction)
    {
        this.pool = pool;
        this.direction = direction.normalized;
        timer = lifespan;
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

        // Check for Enemy first (handles damage and coins)
        if (other.TryGetComponent(out Enemy enemy))
        {
            enemy.TakeDamage(damage, direction);
            SpawnHitMarker(hitPosition);
            ReturnToPool();
        }
        // Check for Hair (handles damage and coin spawn in circle)
        else if (other.TryGetComponent(out Hair hair))
        {
            hair.TakeDamage(damage);
            SpawnHitMarker(hitPosition);
            ReturnToPool();
        }
        // Check for any IHittable object
        else if (other.TryGetComponent(out IHittable hittable))
        {
            hittable.OnHit();
            SpawnHitMarker(hitPosition);
            ReturnToPool();
        }
    }

    private void SpawnHitMarker(Vector3 position)
    {
        if (hitMarkerManager != null)
            hitMarkerManager.SpawnHitMarker(position);
    }

    private void ReturnToPool()
    {
        rb.linearVelocity = Vector2.zero;
        pool.Return(gameObject);
    }
}
