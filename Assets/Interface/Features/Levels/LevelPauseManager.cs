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
    [SerializeField] AttemptManager attemptManager;
    Level _level;

    void Start() {
        _level = StateNameManager.Level;
        _levelName.text = _level.name;
        _editorButton.SetActive(StateNameManager.LatestMainMenuState == MainMenuState.Local);
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
        Debug.Log("[LevelPauseManager]" + StateNameManager.LatestMainMenuState);
        if (StateNameManager.LatestMainMenuState != MainMenuState.Server) return;

        var completion = StateNameManager.LoadedLevelCompletion ?? new Completion();
        completion.percentage = Mathf.Max(completion.percentage, attemptManager.CompletionPercent);
        completion.accuracy = Mathf.Max(completion.accuracy, attemptManager.AccuracyPercent);
        completion.attempts = Mathf.Max(completion.attempts, attemptManager.AttemptCount);

        StateNameManager.LoadedLevelCompletion = completion;

        await SupabaseManager.Instance.Completion.CompleteLevel(StateNameManager.Level.serverId.Value, completion);
    }

    [UsedImplicitly]
    public async void Replay() {
        attemptManager.PersistAttemptForRetry();
        // TODO: might avoid reloading the scene here all together and instead re-initialize the beat-manager
        await SceneStateManager.Reload();
        _pauseManager.Resume();
    }
}
