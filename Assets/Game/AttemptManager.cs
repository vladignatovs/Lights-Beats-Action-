using TMPro;
using UnityEngine;

/// <summary>
/// Class reliable for the base game-cycle of each attempt either dying or completing.
/// Acts as a single source of truth for the gameplay stats and run state.
/// </summary>
public class AttemptManager : MonoBehaviour {
    [Header("Stats Sources")]
    [SerializeField] BaseBeatManager _baseBeatManager;
    [SerializeField] DRManager _dashRadiusManager;

    protected virtual bool UsesGameplayStats => true;

    // level completion marking
    public event System.Action OnLevelCompleted;
    public event System.Action OnDeathEvent;
    public bool HasCompleted { get; private set; }

    // level completion data
    public int AttemptCount { get; private set; }
    public virtual float AccuracyPercent {
        get {
            float elapsedSeconds = _baseBeatManager.ProgressFromOffsetBeats * _baseBeatManager.SecondsPerBeat;
            if (elapsedSeconds <= 0f) return 0f;
            return Mathf.Clamp01(_dashRadiusManager.HoveredTimeSeconds / elapsedSeconds);
        }
    }
    public virtual float CompletionPercent => _baseBeatManager.CompletionPercent;

    void Start() {
        InitializeAttempt();
        ResetRunState();
    }

    void Update() {
        if (UsesGameplayStats && CompletionPercent == 1) CompleteLevel();
    }

    public async void restartGame() {
        PersistAttemptForRetry();
        await SceneStateManager.Reload();
    }

    public void PersistAttemptForRetry() {
        if (StateNameManager.LatestMainMenuState != MainMenuState.Server && StateNameManager.LoadedLevelCompletion == null) {
            return;
        }

        StateNameManager.LoadedLevelCompletion ??= new Completion();
        StateNameManager.LoadedLevelCompletion.attempts = AttemptCount;
    }

    public virtual void gameOver() {
        if (GameStateManager.IsGameOver) return;
        Time.timeScale = 0;
        GameStateManager.IsGameOver = true;
        PauseManager.CanPause = false;
        OnDeathEvent?.Invoke();
    }

    public virtual void CompleteLevel() {
        if (HasCompleted) return;
        GameStateManager.IsLevelCompleted = true;
        HasCompleted = true;
        OnLevelCompleted?.Invoke();
    }

    void InitializeAttempt() {
        int savedAttempts = StateNameManager.LoadedLevelCompletion?.attempts ?? 0;
        AttemptCount = savedAttempts + 1;
    }

    void ResetRunState() {
        GameStateManager.IsGameOver = false;
        GameStateManager.IsLevelCompleted = false;
        PauseManager.CanPause = true;
        Time.timeScale = 1;
        HasCompleted = false;
        Overlay.ToggleOverlay(false);
    }
}
