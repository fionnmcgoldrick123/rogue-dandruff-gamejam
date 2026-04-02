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
    }
}
