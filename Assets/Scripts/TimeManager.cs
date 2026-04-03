using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    [Header("Wave Settings")]
    [SerializeField] private float waveLength = 300f; // 5 minutes in seconds
    private float waveTimer;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        waveTimer = waveLength;
    }

    private void Update()
    {
        // Countdown timer
        if (waveTimer > 0f)
        {
            waveTimer -= Time.deltaTime;
            if (waveTimer < 0f)
                waveTimer = 0f;
        }
    }

    /// <summary>
    /// Get remaining time in the current wave
    /// </summary>
    public float GetRemainingTime()
    {
        return waveTimer;
    }

    /// <summary>
    /// Reset timer to wave length for next wave
    /// </summary>
    public void StartNewWave()
    {
        waveTimer = waveLength;
    }
}
