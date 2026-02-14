using JetBrains.Annotations;
using TMPro;
using UnityEngine;

/// <summary>
/// Class reliable for the UI and logic handling of the editor pause menu, 
/// specific to the <see cref="Scene.Editor"/> Is built to be used alongside PauseManager on a
/// shared prefab representing the editor pause menu.
/// </summary>
public class EditorPauseManager : MonoBehaviour {
    [SerializeField] TMP_Text _levelName;
    [SerializeField] PauseManager _pauseManager;
    [SerializeField] ActionCreator _actionCreator;
    [SerializeField] ConfirmationManager _confirmationManager;
    Level _level;

    void Start() {
        _level = StateNameManager.Level;
        _levelName.text = _level.name;
    }

    [UsedImplicitly]
    public void Publish() { // TODO: provide a success state feedback
        _confirmationManager.ShowConfirmation(
            async () => await _actionCreator.PublishLevel());
    }

    [UsedImplicitly]
    public async void Save() {
        await _actionCreator.SaveAllActions();
    }

    [UsedImplicitly]
    public void Delete() {
        _confirmationManager.ShowConfirmation(
            async () => {
                _actionCreator.DeleteLevel();
                await SceneStateManager.LoadMain();
                _pauseManager.Resume();
            });
    }

    [UsedImplicitly]
    public async void SaveAndPlay() {
        await _actionCreator.SaveAllActions();
        await SceneStateManager.LoadGame();
        _pauseManager.Resume();
    }

    [UsedImplicitly]
    public async void SaveAndGoToMenu() {
        await _actionCreator.SaveAllActions();
        await SceneStateManager.LoadMain();
        _pauseManager.Resume();
    }

    [UsedImplicitly] // TODO: show a confirmation only when exiting without saving
    public void GoToMenu() {
        _confirmationManager.ShowConfirmation(
            async () => {
                await SceneStateManager.LoadMain();
                _pauseManager.Resume();
            });
    }
}
