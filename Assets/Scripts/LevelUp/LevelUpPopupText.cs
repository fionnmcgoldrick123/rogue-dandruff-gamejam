using UnityEngine;
using TMPro;

public class LevelUpPopupText : MonoBehaviour
{
    [SerializeField] private float lifetime = 1.5f;
    [SerializeField] private float floatSpeed = 150f;
    [SerializeField] private float fadeDelay = 0.6f;

    private TMP_Text tmp;
    private RectTransform rectTransform;
    private float timer;

    private void Awake()
    {
        tmp = GetComponentInChildren<TMP_Text>();
        rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        timer = 0f;
        if (tmp != null)
            tmp.color = new Color(tmp.color.r, tmp.color.g, tmp.color.b, 1f);
    }

    public void Init(Vector3 playerWorldPos, Camera mainCamera)
    {
        if (mainCamera == null) mainCamera = Camera.main;

        // Convert world position to screen space, then to canvas local position
        Vector3 screenPos = mainCamera.WorldToScreenPoint(playerWorldPos);
        screenPos.y += 80f; // offset above player in screen pixels
        rectTransform.position = screenPos;
    }

    private void Update()
    {
        timer += Time.unscaledDeltaTime;

        // Float upward in screen space
        rectTransform.position += Vector3.up * floatSpeed * Time.unscaledDeltaTime;

        if (timer > fadeDelay)
        {
            float t = (timer - fadeDelay) / (lifetime - fadeDelay);
            Color c = tmp.color;
            c.a = Mathf.Clamp01(1f - t);
            tmp.color = c;
        }

        if (timer >= lifetime)
            Destroy(gameObject);
    }
}