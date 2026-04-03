using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        float velocityMagnitude = rb.linearVelocity.magnitude;
        
        if (velocityMagnitude > 0)
        {
            animator.SetBool("IsMoving", true);
        }
        else
        {
            animator.SetBool("IsMoving", false);
        }
        
        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (mouseWorld.x < transform.position.x)
        {
            spriteRenderer.flipX = true;
        }
        else if (mouseWorld.x > transform.position.x)
        {
            spriteRenderer.flipX = false;
        }
        
        // Check for moonwalking: moving opposite direction from facing
        bool isMoonwalking = false;
        if (rb.linearVelocity.x != 0)
        {
            // Moving right but facing left (flipX = true)
            if (rb.linearVelocity.x > 0 && spriteRenderer.flipX)
            {
                isMoonwalking = true;
            }
            // Moving left but facing right (flipX = false)
            else if (rb.linearVelocity.x < 0 && !spriteRenderer.flipX)
            {
                isMoonwalking = true;
            }
        }
        animator.SetBool("IsMoonWalking", isMoonwalking);
    }
}
