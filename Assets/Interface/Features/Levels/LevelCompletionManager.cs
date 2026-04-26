using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

public class LevelCompletionManager : MonoBehaviour {
    [SerializeField] TMP_Text _levelName;
    [SerializeField] GameObject _editorButton;
    [SerializeField] GameObject _statsPanel;
    [SerializeField] TMP_Text _completionText;
    [SerializeField] TMP_Text _accuracyText;
    [SerializeField] TMP_Text _attemptCountText;
    [SerializeField] AttemptManager _attemptManager;
    Level _level;

    void Awake() {
        _level = StateNameManager.Level;
    }

    void OnEnable() {
        _levelName.text = _level.name;
        bool isServerLevel = StateNameManager.LatestMainMenuState == MainMenuState.Server;

        _editorButton.SetActive(StateNameManager.LatestMainMenuState == MainMenuState.Local);
        _statsPanel.SetActive(isServerLevel);

        if (isServerLevel) {
            RefreshServerStats();
        }
    }

    [UsedImplicitly]
    public async void GoToEditor() {
        if (StateNameManager.LatestMainMenuState != MainMenuState.Local) {
            return;
        }

        Time.timeScale = 1;
        Overlay.ToggleOverlay(false);
        await SceneStateManager.LoadEditor();
    }

    [UsedImplicitly]
    public async void GoToMenu() {
        await TryPersistServerLevelCompletion();

        var player = FindAnyObjectByType<PlayerMovement>();
        if (player != null) {
            StateNameManager.PlayerPosition = player.transform.position;
        } else {
            Debug.LogWarning("No PlayerMovement found in current scene.");
        }

        Time.timeScale = 1;
        Overlay.ToggleOverlay(false);
        PauseManager.CanPause = true;
        await SceneStateManager.LoadMain();
    }

    [UsedImplicitly]
    public async void Replay() {
        await TryPersistServerLevelCompletion();
        _attemptManager.PersistAttemptForRetry();
        PauseManager.CanPause = true;
        await SceneStateManager.Reload();
    }

    async Task TryPersistServerLevelCompletion() {
        if (StateNameManager.LatestMainMenuState != MainMenuState.Server) {
            return;
        }

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
        completion.percentage = Mathf.Max(completion.percentage, _attemptManager.CompletionPercent);
        completion.accuracy = Mathf.Max(completion.accuracy, _attemptManager.AccuracyPercent);
        completion.attempts = Mathf.Max(completion.attempts, _attemptManager.AttemptCount);
        return completion;
    }
}
