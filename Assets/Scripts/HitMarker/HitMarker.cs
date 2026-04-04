using UnityEngine;
using UnityEngine.UI;

public class HitMarker : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float lifetime = 0.3f;
    [SerializeField] private float fadeSpeed = 3f;
    [SerializeField] private float scaleSpeed = 2f;
    [SerializeField] private float targetScale = 1.5f;

    private CanvasGroup canvasGroup;
    private Vector3 startScale;
    private float timer;
    private ObjectPool pool;

    private void Awake()
    {
        canvasGroup = GetComponentInChildren<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        startScale = transform.localScale;
    }

    private void OnEnable()
    {
        timer = 0f;
        canvasGroup.alpha = 1f;
        transform.localScale = startScale;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        // Scale up on spawn
        transform.localScale = Vector3.Lerp(
            transform.localScale,
            startScale * targetScale,
            Time.deltaTime * scaleSpeed
        );

        // Fade out near end of lifetime
        if (timer > lifetime * 0.5f)
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 0f, Time.deltaTime * fadeSpeed);

        if (timer >= lifetime)
        {
            if (pool != null)
                pool.Return(gameObject);
            else
                gameObject.SetActive(false);
        }
    }

    public void Init(ObjectPool pool)
    {
        this.pool = pool;
    }
}
