using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    [SerializeField] private float magnetRadius = 3f;
    [SerializeField] private LayerMask coinLayer;

    private int cash;

    public int Cash => cash;

    private void Update()
    {
        Collider2D[] coins = Physics2D.OverlapCircleAll(transform.position, magnetRadius, coinLayer);
        for (int i = 0; i < coins.Length; i++)
        {
            if (coins[i].TryGetComponent(out Coin coin))
                coin.StartCollecting();
        }
    }

    public void AddCash(int amount)
    {
        cash += amount;
    }
}
