using System;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// The script reliable for the general pause state management, doesn't handle the ui and is supposed
/// to be easily dropped on a prefab in order to make it functional as a pause menu
/// </summary>
public class PauseManager : MonoBehaviour {
    [SerializeField] CanvasGroup _canvasGroup;
    public static bool IsPaused { get; private set; } = false;
    public static bool CanPause { get; set; } = true;
    public KeyCode PauseBind { get; private set; } = KeyCode.Escape; // TODO: make pause bind button configurable
    public event Action<bool> OnPauseChanged;

    void Update() {
        if(Input.GetKeyDown(PauseBind)) TogglePause(!IsPaused);
    }

    public void Pause() {
        TogglePause(true);
    }

    [UsedImplicitly]
    public void Resume() {
        TogglePause(false);
    }

    /// <summary>
    /// Public method which toggles the state of the pause
    /// </summary>
    /// <param name="state">The state of the pause to set to</param>
    public void TogglePause(bool state) {
        if(state && !CanPause) return;
        IsPaused = state;
        Overlay.ToggleOverlay(state);
        Time.timeScale = state ? 0 : 1;
        ToggleCanvasGroup(state);
        OnPauseChanged?.Invoke(IsPaused);
    }

    /// <summary>
    /// Internal method which toggles the actual panel
    /// </summary>
    void ToggleCanvasGroup(bool state) {
        if(_canvasGroup) {
            _canvasGroup.alpha = state ? 1 : 0;
            _canvasGroup.interactable = state;
            _canvasGroup.blocksRaycasts = state;
        }
    }
}