using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }
    
    [Header("Shake Settings")]
    [SerializeField] private float shakeMagnitude = 0.5f;
    [SerializeField] private float shakeDuration = 0.1f;
    
    private Coroutine shakeRoutine;
    private Vector3 shakeOffset = Vector3.zero;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void LateUpdate()
    {
        // Apply shake offset after all position updates
        transform.position += shakeOffset;
    }

    public void Shake()
    {
        if (shakeRoutine != null)
            StopCoroutine(shakeRoutine);
        shakeRoutine = StartCoroutine(ShakeRoutine());
    }
    
    public void Shake(float magnitude, float duration)
    {
        if (shakeRoutine != null)
            StopCoroutine(shakeRoutine);
        shakeRoutine = StartCoroutine(ShakeRoutine(magnitude, duration));
    }

    private IEnumerator ShakeRoutine()
    {
        return ShakeRoutine(shakeMagnitude, shakeDuration);
    }

    private IEnumerator ShakeRoutine(float magnitude, float duration)
    {
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            
            shakeOffset = new Vector3(x, y, 0f);
            elapsed += Time.deltaTime;
            
            yield return null;
        }
        
        shakeOffset = Vector3.zero;
        shakeRoutine = null;
    }
}
