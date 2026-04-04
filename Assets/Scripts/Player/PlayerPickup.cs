using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    [SerializeField] private float magnetRadius = 3f;
    [SerializeField] private LayerMask expOrbLayer;

    private void Update()
    {
        Collider2D[] orbs = Physics2D.OverlapCircleAll(transform.position, magnetRadius, expOrbLayer);
        for (int i = 0; i < orbs.Length; i++)
        {
            if (orbs[i].TryGetComponent(out ExpOrb orb))
                orb.StartCollecting();
        }
    }

    public void AddExp(int amount)
    {
        if (LevelManager.Instance != null)
            LevelManager.Instance.AddExp(amount);
    }
}