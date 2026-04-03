using System.Collections;
using UnityEngine;

public class HitFlash : MonoBehaviour
{
    [Header("Material Settings")]
    [SerializeField] private Material whiteMaterial;
    
    [Header("Scale Settings")]
    [SerializeField] private float scaleMultiplier = 1.1f;
    [SerializeField] private float scaleDuration = 0.1f;
    
    [Header("Material Duration")]
    [SerializeField] private float materialDuration = 0.1f;

    private SpriteRenderer spriteRenderer;
    private Material originalMaterial;
    private Vector3 originalScale;
    private Coroutine materialRoutine;
    private Coroutine scaleRoutine;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalMaterial = spriteRenderer.material;
        }
        originalScale = transform.localScale;
    }

    /// <summary>
    /// Trigger the hit flash effect (white material + scale)
    /// </summary>
    public void TriggerHit()
    {
        // Apply white material
        if (whiteMaterial != null)
        {
            if (materialRoutine != null)
                StopCoroutine(materialRoutine);
            materialRoutine = StartCoroutine(MaterialFlashRoutine());
        }

        // Scale up and down
        if (scaleRoutine != null)
            StopCoroutine(scaleRoutine);
        scaleRoutine = StartCoroutine(ScaleRoutine());
    }

    private IEnumerator MaterialFlashRoutine()
    {
        if (spriteRenderer == null)
            yield break;

        spriteRenderer.material = whiteMaterial;
        yield return new WaitForSeconds(materialDuration);
        spriteRenderer.material = originalMaterial;
        materialRoutine = null;
    }

    private IEnumerator ScaleRoutine()
    {
        float elapsed = 0f;
        Vector3 targetScale = originalScale * scaleMultiplier;

        // Scale up
        while (elapsed < scaleDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / scaleDuration;
            transform.localScale = Vector3.Lerp(originalScale, targetScale, progress);
            yield return null;
        }

        // Scale back down
        elapsed = 0f;
        while (elapsed < scaleDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / scaleDuration;
            transform.localScale = Vector3.Lerp(targetScale, originalScale, progress);
            yield return null;
        }

        transform.localScale = originalScale;
        scaleRoutine = null;
    }
}
