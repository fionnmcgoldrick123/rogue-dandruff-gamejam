using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [SerializeField] private int baseExpPerLevel = 100;

    public int CurrentLevel { get; private set; } = 1;
    public int CurrentExp { get; private set; } = 0;
    public int ExpToNextLevel => Mathf.RoundToInt(baseExpPerLevel * CurrentLevel);

    public event System.Action OnLevelUp;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void AddExp(int amount)
    {
        CurrentExp += amount;
        while (CurrentExp >= ExpToNextLevel)
        {
            CurrentExp -= ExpToNextLevel;
            CurrentLevel++;
            OnLevelUp?.Invoke();
            if (GameStateManager.Instance != null)
                GameStateManager.Instance.EnterLevelUp();
        }
    }

    public float GetExpProgress()
    {
        return (float)CurrentExp / ExpToNextLevel;
    }
}