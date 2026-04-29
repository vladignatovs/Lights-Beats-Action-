using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

/// <summary>
/// Class reliable for the UI and logic handling of the level pause menu, 
/// specific to the <see cref="Scene.Game"/>. Is built to be used alongside PauseManager on a
/// shared prefab representing the level pause menu.
/// </summary>
public class LevelPauseManager : MonoBehaviour {
    [SerializeField] TMP_Text _levelName;
    [SerializeField] PauseManager _pauseManager;
    [SerializeField] GameObject _editorButton;
    [SerializeField] GameObject _serverStatsPanel;
    [SerializeField] TMP_Text _completionText;
    [SerializeField] TMP_Text _accuracyText;
    [SerializeField] TMP_Text _attemptCountText;
    [SerializeField] AttemptManager attemptManager;
    Level _level;

    void Start() {
        _level = StateNameManager.Level;
        _levelName.text = _level.name;
        _pauseManager.OnPauseChanged += HandlePauseChanged;

        bool isLocalLevel = StateNameManager.LatestMainMenuState == MainMenuState.Local;
        bool isServerLevel = StateNameManager.LatestMainMenuState == MainMenuState.Server;

        _editorButton.SetActive(isLocalLevel);
        _serverStatsPanel.SetActive(isServerLevel);

        if (isServerLevel) {
            RefreshServerStats();
        }
    }

    void OnDestroy() {
        _pauseManager.OnPauseChanged -= HandlePauseChanged;
    }

    void HandlePauseChanged(bool isPaused) {
        if (!isPaused || StateNameManager.LatestMainMenuState != MainMenuState.Server) {
            return;
        }

        RefreshServerStats();
    }

    [UsedImplicitly]
    public async void GoToEditor() {
        if (StateNameManager.LatestMainMenuState != MainMenuState.Local) return;
        await SceneStateManager.LoadEditor();
        _pauseManager.Resume();
    }

    [UsedImplicitly]
    public async void GoToMenu() {
        await TryPersistServerLevelCompletion();

        var player = FindAnyObjectByType<PlayerMovement>();
        if (player != null) 
            StateNameManager.PlayerPosition = player.transform.position;
        else 
            Debug.LogWarning("No PlayerMovement found in current scene.");
        await SceneStateManager.LoadMain();
        _pauseManager.Resume();
    }

    async Task TryPersistServerLevelCompletion() {
        if (StateNameManager.LatestMainMenuState != MainMenuState.Server) return;

        var completion = GetCurrentServerCompletion();

        StateNameManager.LoadedLevelCompletion = completion;

        await SupabaseManager.Instance.Completion.CompleteLevel(StateNameManager.Level.serverId.Value, completion);
    }

    void RefreshServerStats() {
        var completion = GetCurrentServerCompletion();
        float completionPercent = Mathf.Clamp01(completion.percentage);
        float accuracyPercent = Mathf.Clamp01(completion.accuracy);

        _completionText.text = $"Completion: {completionPercent:P0}";
        _accuracyText.text = $"Accuracy: {accuracyPercent:P0}";
        _attemptCountText.text = $"Attempts: {completion.attempts}";
    }

    Completion GetCurrentServerCompletion() {
        var completion = StateNameManager.LoadedLevelCompletion ?? new Completion();
        completion.percentage = Mathf.Max(completion.percentage, attemptManager.CompletionPercent);
        completion.accuracy = Mathf.Max(completion.accuracy, attemptManager.AccuracyPercent);
        completion.attempts = Mathf.Max(completion.attempts, attemptManager.AttemptCount);
        return completion;
    }

    [UsedImplicitly]
    public async void Replay() {
        attemptManager.PersistAttemptForRetry();
        await SceneStateManager.Reload();
        _pauseManager.Resume();
    }
}
