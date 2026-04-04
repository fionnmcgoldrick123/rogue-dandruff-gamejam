using UnityEngine;
using TMPro;

public class DamageNumber : MonoBehaviour
{
    [SerializeField] private float lifetime = 1f;
    [SerializeField] private float floatSpeed = 1.5f;
    [SerializeField] private float fadeDelay = 0.4f;
    [SerializeField] private float normalFontSize = 28f;
    [SerializeField] private float critFontSize = 42f;

    private TextMeshProUGUI tmp;
    private RectTransform textRect;
    private ObjectPool pool;
    private float timer;
    private Vector3 worldPosition;
    private Camera mainCamera;

    private void Awake()
    {
        tmp = GetComponentInChildren<TextMeshProUGUI>();
        textRect = tmp.GetComponent<RectTransform>();
        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        timer = 0f;
        if (tmp != null)
        {
            Color c = tmp.color;
            c.a = 1f;
            tmp.color = c;
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;

        // Float upward in world space then project to screen
        worldPosition += Vector3.up * floatSpeed * Time.deltaTime;
        textRect.position = mainCamera.WorldToScreenPoint(worldPosition);

        // Fade out near end of lifetime
        if (timer > fadeDelay)
        {
            float t = (timer - fadeDelay) / (lifetime - fadeDelay);
            Color c = tmp.color;
            c.a = Mathf.Clamp01(1f - t);
            tmp.color = c;
        }

        if (timer >= lifetime)
        {
            if (pool != null)
                pool.Return(gameObject);
            else
                gameObject.SetActive(false);
        }
    }

    public void Init(Vector3 worldPos, float damage, bool isCrit, Color normalColor, Color critColor, ObjectPool pool)
    {
        this.pool = pool;
        this.worldPosition = worldPos;

        if (mainCamera == null)
            mainCamera = Camera.main;

        tmp.text = Mathf.CeilToInt(damage).ToString();
        tmp.color = isCrit ? critColor : normalColor;
        tmp.fontSize = isCrit ? critFontSize : normalFontSize;

        textRect.position = mainCamera.WorldToScreenPoint(worldPosition);
    }
}