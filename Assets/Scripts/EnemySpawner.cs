using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawning Settings")]
    public GameObject[] enemyPrefabs; 
    public float spawnInterval = 1f;            
    public float spawnDistance = 12f;           
    public float spawnRadiusVariation = 2f;     

    private Transform player;
    private Camera mainCam;
    private float spawnTimer;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        mainCam = Camera.main;
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
        if (enemyPrefabs.Length == 0) return;

        GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

        float angle = Random.Range(0f, 360f);
        Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

        float distance = spawnDistance + Random.Range(-spawnRadiusVariation, spawnRadiusVariation);

        Vector3 spawnPosition = player.position + (Vector3)(direction * distance);
        spawnPosition.z = 0f;

        Instantiate(prefab, spawnPosition, Quaternion.identity);
    }
}