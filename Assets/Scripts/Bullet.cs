using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float lifespan;
    [SerializeField] private int damage;

    private Rigidbody2D rb;

    private void Start()
    {
        Destroy(gameObject, lifespan);
    }

    private void FixedUpdate()
    {
        GetComponent<Rigidbody2D>().linearVelocity = transform.right * speed;
    }
}
