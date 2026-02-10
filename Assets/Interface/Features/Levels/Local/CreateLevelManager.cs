using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using JetBrains.Annotations;

public class CreateLevelManager : MonoBehaviour{
    [SerializeField] GameObject _createLevelPanel;
    [SerializeField] InputField _levelNameInput;
    [SerializeField] InputField _bpmInput;
    [SerializeField] TMP_Dropdown _audioDropdown;
    [SerializeField] Button _createButton;

    [UsedImplicitly]
    public async void CreateLevel() {
        await LocalLevelManager.CreateNewLevel(
            _levelNameInput.text,
            _bpmInput.text.FloatParse(),
            _audioDropdown.options[_audioDropdown.value].text
        );
        // reload the scene after creating a new level to update the list of levels
        // TODO: a MUCH better result would be if i reloaded the current page
        await SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
    }

    [UsedImplicitly]
    public void ToggleCreateLevelPanel() {
        var state = !_createLevelPanel.activeSelf;
        _createLevelPanel.SetActive(state);
        Overlay.ToggleOverlay(state);
    }

    [UsedImplicitly]
    public void VerifyInputs() {
        _createButton.interactable = _levelNameInput.text.Length > 0 && _bpmInput.text.Length > 0 && _audioDropdown.value != 0;
    }
}
