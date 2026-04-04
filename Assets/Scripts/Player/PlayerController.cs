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
        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        if (moveInput != Vector2.zero)
        {
            // Accelerate toward input direction
            currentVelocity = Vector2.MoveTowards(
                currentVelocity,
                moveInput * moveSpeed,
                acceleration * Time.fixedDeltaTime
            );
        }
        else
        {
            // Decelerate to zero when no input
            currentVelocity = Vector2.MoveTowards(
                currentVelocity,
                Vector2.zero,
                deceleration * Time.fixedDeltaTime
            );
        }

        rb.linearVelocity = currentVelocity;
    }
}