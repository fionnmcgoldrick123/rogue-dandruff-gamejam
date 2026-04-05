using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Screen-space arrow that points toward the nearest active Hair.
/// Attach to a Canvas child. Assign arrowRect to your arrow Image's RectTransform.
/// The arrow Image sprite should point UP (0°) so rotation math works correctly.
/// </summary>
public class HairArrowUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform arrowRect;
    [SerializeField] private Canvas canvas;

    [Header("Settings")]
    [Tooltip("Distance from screen edge (pixels).")]
    [SerializeField] private float edgePadding = 70f;
    [Tooltip("How often (seconds) to re-scan for active hairs.")]
    [SerializeField] private float refreshInterval = 0.5f;
    [Tooltip("Hide arrow when the target hair is visible on screen.")]
    [SerializeField] private bool hideWhenOnScreen = true;

    private Camera mainCamera;
    private Hair[] cachedHairs = new Hair[0];
    private float refreshTimer;
    private Transform playerTransform;

    // ── Unity ─────────────────────────────────────────────────────────────────

    private void Start()
    {
        mainCamera = Camera.main;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) playerTransform = playerObj.transform;

        RefreshHairList();
    }

    private void Update()
    {
        refreshTimer -= Time.unscaledDeltaTime;
        if (refreshTimer <= 0f)
        {
            RefreshHairList();
            refreshTimer = refreshInterval;
        }

        Hair nearest = GetNearestHair();
        if (nearest == null)
        {
            arrowRect.gameObject.SetActive(false);
            return;
        }

        UpdateArrow(nearest.transform.position);
    }

    // ── Hair tracking ─────────────────────────────────────────────────────────

    private void RefreshHairList()
    {
        cachedHairs = Object.FindObjectsByType<Hair>(FindObjectsSortMode.None);
    }

    private Hair GetNearestHair()
    {
        Vector3 referencePos = playerTransform != null
            ? playerTransform.position
            : (mainCamera != null ? mainCamera.transform.position : Vector3.zero);

        Hair nearest = null;
        float nearestDist = float.MaxValue;

        foreach (var hair in cachedHairs)
        {
            if (hair == null || !hair.gameObject.activeSelf) continue;
            float dist = Vector3.SqrMagnitude(hair.transform.position - referencePos);
            if (dist < nearestDist)
            {
                nearestDist = dist;
                nearest = hair;
            }
        }

        return nearest;
    }

    // ── Arrow rendering ───────────────────────────────────────────────────────

    private void UpdateArrow(Vector3 targetWorldPos)
    {
        Vector3 screenPos = mainCamera.WorldToScreenPoint(targetWorldPos);

        bool isOnScreen = screenPos.z > 0f
            && screenPos.x > 0f && screenPos.x < Screen.width
            && screenPos.y > 0f && screenPos.y < Screen.height;

        if (isOnScreen && hideWhenOnScreen)
        {
            arrowRect.gameObject.SetActive(false);
            return;
        }

        arrowRect.gameObject.SetActive(true);

        Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        Vector2 dir = (Vector2)screenPos - screenCenter;

        // Clamp position to screen edges with padding
        float halfW = Screen.width  * 0.5f - edgePadding;
        float halfH = Screen.height * 0.5f - edgePadding;
        Vector2 normDir = dir.normalized;
        float scaleX = halfW / Mathf.Abs(normDir.x);
        float scaleY = halfH / Mathf.Abs(normDir.y);
        Vector2 clampedPos = screenCenter + normDir * Mathf.Min(scaleX, scaleY);

        // Apply position (works for both Screen Space Overlay and Camera canvas)
        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            arrowRect.position = clampedPos;
        }
        else
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.GetComponent<RectTransform>(),
                clampedPos,
                canvas.worldCamera,
                out Vector2 localPos);
            arrowRect.anchoredPosition = localPos;
        }

        // Rotate arrow to point toward target (sprite assumed to point UP)
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        arrowRect.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}
