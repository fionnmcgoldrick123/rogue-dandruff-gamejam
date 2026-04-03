using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayMuzzleFlash()
    {
        if (animator != null)
        {
            animator.SetTrigger("Flash");
        }
    }
}
