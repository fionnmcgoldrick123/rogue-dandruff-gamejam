using UnityEngine;

public class HitMarkerManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ObjectPool hitMarkerPool;

    [Header("Settings")]
    [SerializeField] private float markerZOffset = 0.1f;

    public void SpawnHitMarker(Vector3 position)
    {
        if (hitMarkerPool == null)
        {
            Debug.LogWarning("HitMarkerPool not assigned to HitMarkerManager!");
            return;
        }

        // Set z to put marker in front of game objects
        position.z = markerZOffset;

        // Get marker from pool
        GameObject markerObj = hitMarkerPool.Get(position, Quaternion.identity);
        
        // Try to find HitMarker component on root or children
        HitMarker marker = markerObj.GetComponent<HitMarker>();
        if (marker == null)
            marker = markerObj.GetComponentInChildren<HitMarker>();
        
        if (marker == null)
        {
            Debug.LogError($"HitMarker component not found on '{markerObj.name}' or its children! " +
                "Make sure the HitMarker.cs script is attached to the prefab or its children.");
            hitMarkerPool.Return(markerObj);
            return;
        }

        marker.Init(hitMarkerPool);
    }
}

