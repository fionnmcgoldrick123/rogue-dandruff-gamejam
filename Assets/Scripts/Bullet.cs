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

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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
        // Check for Enemy first (handles damage and coins)
        if (other.TryGetComponent(out Enemy enemy))
        {
            enemy.TakeDamage(damage, direction);
            ReturnToPool();
        }
        // Check for any IHittable object
        else if (other.TryGetComponent(out IHittable hittable))
        {
            hittable.OnHit();
            ReturnToPool();
        }
    }

    private void ReturnToPool()
    {
        rb.linearVelocity = Vector2.zero;
        pool.Return(gameObject);
    }
}
