using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TopDownMovement : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private PlayerStats stats;

    [Header("Movement Fallback")]
    public float moveSpeed = 5f;
    public float acceleration = 50f;
    public float deceleration = 50f;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 currentVelocity;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        if (stats != null)
        {
            moveSpeed = stats.moveSpeed;
            acceleration = stats.acceleration;
            deceleration = stats.deceleration;
        }
    }

    void Update()
    {
        if (GameStateManager.Instance != null && GameStateManager.Instance.CurrentState == GameState.LevellingUp)
        {
            moveInput = Vector2.zero;
            return;
        }
        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        // Read live from stats so upgrades take effect immediately
        float speed = stats != null ? stats.moveSpeed : moveSpeed;
        float accel = stats != null ? stats.acceleration : acceleration;
        float decel = stats != null ? stats.deceleration : deceleration;

        if (moveInput != Vector2.zero)
        {
            currentVelocity = Vector2.MoveTowards(
                currentVelocity,
                moveInput * speed,
                accel * Time.fixedDeltaTime
            );
        }
        else
        {
            currentVelocity = Vector2.MoveTowards(
                currentVelocity,
                Vector2.zero,
                decel * Time.fixedDeltaTime
            );
        }

        rb.linearVelocity = currentVelocity;
    }
}