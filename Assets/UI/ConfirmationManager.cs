using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script manager for the ConfirmationPanel prefab that is built to be reusable alongside the panel for a 
/// simple confirmation of any action.
/// </summary>
public class ConfirmationManager : MonoBehaviour {
    private System.Action _onConfirm;
    private bool _initialized;

    public void ShowConfirmation(System.Action onConfirm) {
        if (!_initialized) {
            transform.Find("YesButton").GetComponent<Button>().onClick.AddListener(OnYesPressed);
            transform.Find("NoButton").GetComponent<Button>().onClick.AddListener(OnNoPressed);
            _initialized = true;
        }
        _onConfirm = onConfirm;
        gameObject.SetActive(true);
    }

    private void OnYesPressed() {
        _onConfirm?.Invoke();
        gameObject.SetActive(false);
    }

    private void OnNoPressed() {
        gameObject.SetActive(false);
    }
}