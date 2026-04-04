using UnityEngine;

public enum GameState { Combat, LevellingUp }

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    public GameState CurrentState { get; private set; } = GameState.Combat;

    public event System.Action<GameState> OnStateChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void EnterCombat() => SetState(GameState.Combat);
    public void EnterLevelUp() => SetState(GameState.LevellingUp);

    private void SetState(GameState state)
    {
        CurrentState = state;
        OnStateChanged?.Invoke(state);
    }
}