using UnityEngine;

public class ExpOrb : MonoBehaviour
{
    [SerializeField] private float launchSpeed = 6f;
    [SerializeField] private float deceleration = 8f;
    [SerializeField] private float magnetSpeed = 12f;
    [SerializeField] private float pickupDistance = 0.3f;
    [SerializeField] private float interactableCooldown = 0.5f;
    [SerializeField] private int expValue = 1;

    private Vector2 velocity;
    private ObjectPool pool;
    private Transform player;
    private bool beingCollected;
    private bool canBeCollected;

    public void Launch(Vector2 direction, ObjectPool pool)
    {
        CancelInvoke(nameof(EnableInteraction));
        canBeCollected = false;
        Invoke(nameof(EnableInteraction), interactableCooldown);

        this.pool = pool;
        velocity = direction.normalized * launchSpeed;
        beingCollected = false;

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    private void Update()
    {
        if (beingCollected)
        {
            if (player == null) return;

            Vector2 toPlayer = (Vector2)player.position - (Vector2)transform.position;
            float dist = toPlayer.magnitude;

            if (dist <= pickupDistance)
            {
                Pickup();
                return;
            }

            transform.position += (Vector3)(toPlayer.normalized * magnetSpeed * Time.deltaTime);
        }
        else
        {
            velocity = Vector2.MoveTowards(velocity, Vector2.zero, deceleration * Time.deltaTime);
            transform.position += (Vector3)(velocity * Time.deltaTime);
        }
    }

    private void EnableInteraction()
    {
        canBeCollected = true;
    }

    public void StartCollecting()
    {
        if (canBeCollected)
            beingCollected = true;
    }

    private void Pickup()
    {
        if (player != null && player.TryGetComponent(out PlayerPickup pickup))
            pickup.AddExp(expValue);

        pool.Return(gameObject);
    }
}