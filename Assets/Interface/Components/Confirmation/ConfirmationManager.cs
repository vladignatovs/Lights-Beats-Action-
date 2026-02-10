using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script manager for the ConfirmationPanel prefab that is built to be reusable alongside the panel for a 
/// simple confirmation of any action.
/// </summary> // TODO: use this in the level editor after revamping the pause
public class ConfirmationManager : MonoBehaviour {
    [SerializeField] Button _confirmButton;
    [SerializeField] Button _cancelButton;
    private System.Action _onConfirm;

    // TODO: could also pass custom text to the confirmation?
    public void ShowConfirmation(System.Action onConfirm) {
        _onConfirm = onConfirm;
        Overlay.ToggleOverlay(true);
        gameObject.SetActive(true);
    }

    public void OnConfirmPressed() {
        _onConfirm?.Invoke();
        Overlay.ToggleOverlay(false);
        gameObject.SetActive(false);
    }

    public void OnCancelPressed() {
        Overlay.ToggleOverlay(false);
        gameObject.SetActive(false);
    }
}