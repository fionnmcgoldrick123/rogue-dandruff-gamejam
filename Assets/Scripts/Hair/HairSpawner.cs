using System.Collections.Generic;
using UnityEngine;

public class HairSpawner : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private GameObject hairPrefab;

    [Header("Spawn Area")]
    [SerializeField] private float minSpawnRadius = 5f;
    [SerializeField] private float maxSpawnRadius = 20f;
    /// <summary>Leave unassigned to spawn around world origin.</summary>
    [SerializeField] private Transform spawnCenter;

    [Header("Hair Count Per Wave")]
    [Tooltip("Index 0 = wave 1, index 1 = wave 2, etc. Last entry is used for any wave beyond the array.")]
    [SerializeField] private int[] hairCountsPerWave = { 3, 4, 5, 6, 7, 8, 9, 10, 12, 15 };

    private readonly List<GameObject> spawnedHairs = new List<GameObject>();

    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>Spawns hairs for the given wave index (0-based).</summary>
    public void SpawnForWave(int waveIndex)
    {
        CleanDeadHairs();

        int count = waveIndex < hairCountsPerWave.Length
            ? hairCountsPerWave[waveIndex]
            : hairCountsPerWave[hairCountsPerWave.Length - 1];

        Vector3 center = spawnCenter != null ? spawnCenter.position : Vector3.zero;

        for (int i = 0; i < count; i++)
        {
            Vector3 pos = RandomPositionAround(center);
            GameObject hair = Instantiate(hairPrefab, pos, Quaternion.identity);
            spawnedHairs.Add(hair);
        }
    }

    /// <summary>Destroys all tracked hairs (e.g. on round reset).</summary>
    public void ClearAllHairs()
    {
        foreach (var h in spawnedHairs)
        {
            if (h != null)
                Destroy(h);
        }
        spawnedHairs.Clear();
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private Vector3 RandomPositionAround(Vector3 center)
    {
        float angle = Random.Range(0f, Mathf.PI * 2f);
        float dist  = Random.Range(minSpawnRadius, maxSpawnRadius);
        return center + new Vector3(Mathf.Cos(angle) * dist, Mathf.Sin(angle) * dist, 0f);
    }

    private void CleanDeadHairs()
    {
        spawnedHairs.RemoveAll(h => h == null || !h.activeSelf);
    }

    // ── Editor helpers ────────────────────────────────────────────────────────

    [ContextMenu("Test Spawn Wave 0")]
    private void TestSpawnWave0() => SpawnForWave(0);

    [ContextMenu("Clear All Hairs")]
    private void EditorClear() => ClearAllHairs();
}
