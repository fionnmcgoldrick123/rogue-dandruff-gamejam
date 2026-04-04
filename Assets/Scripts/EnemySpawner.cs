using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawning Settings")]
    public float spawnInterval = 1f;
    public float spawnDistance = 12f;
    public float spawnRadiusVariation = 2f;

    [Header("Pools")]
    [SerializeField] private ObjectPool enemyPool;
    [SerializeField] private ObjectPool expOrbPool;

    private Transform player;
    private float spawnTimer;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f)
        {
            SpawnEnemy();
            spawnTimer = spawnInterval;
        }
    }

    void SpawnEnemy()
    {
        float angle = Random.Range(0f, 360f);
        Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
        float distance = spawnDistance + Random.Range(-spawnRadiusVariation, spawnRadiusVariation);

        Vector3 spawnPosition = player.position + (Vector3)(direction * distance);
        spawnPosition.z = 0f;

        GameObject enemy = enemyPool.Get(spawnPosition, Quaternion.identity);
        enemy.GetComponent<Enemy>().Init(enemyPool, expOrbPool);
    }
}