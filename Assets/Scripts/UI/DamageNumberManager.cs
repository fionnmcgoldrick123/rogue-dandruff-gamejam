using UnityEngine;

public class DamageNumberManager : MonoBehaviour
{
    public static DamageNumberManager Instance { get; private set; }

    [SerializeField] private ObjectPool damageNumberPool;
    [SerializeField] private PlayerStats playerStats;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SpawnDamageNumber(Vector3 worldPosition, float damage, bool isCrit)
    {
        if (damageNumberPool == null) return;

        // Small random horizontal offset so numbers dont stack
        Vector3 spawnPos = worldPosition;
        spawnPos.x += Random.Range(-0.15f, 0.15f);
        spawnPos.y += 0.4f;

        GameObject obj = damageNumberPool.Get(Vector3.zero, Quaternion.identity);
        DamageNumber number = obj.GetComponent<DamageNumber>();

        if (number == null)
        {
            damageNumberPool.Return(obj);
            return;
        }

        Color normal = playerStats != null ? playerStats.normalDamageColor : Color.white;
        Color crit = playerStats != null ? playerStats.critColor : Color.yellow;
        number.Init(spawnPos, damage, isCrit, normal, crit, damageNumberPool);
    }
}