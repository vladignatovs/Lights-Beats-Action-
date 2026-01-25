using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CreateButtonManager : MonoBehaviour
{
    [SerializeField] InputField _levelNameInput;
    [SerializeField] InputField _bpmInput;
    [SerializeField] Dropdown _audioDropdown;
    Button _createButton;
    void Start()
    {
        _createButton = GetComponent<Button>();
        _createButton.onClick.AddListener(CreateLevel);
    }
    
    async void CreateLevel() {
        await LevelManager.CreateNewLevel(
            _levelNameInput.text,
            _bpmInput.text.FloatParse(),
            "Audio/" + _audioDropdown.options[_audioDropdown.value].text
        );
        SceneManager.LoadScene("LevelSelect");
    }

    public void VerifyInputs() {
        _createButton.interactable = _levelNameInput.text.Length > 0 && _bpmInput.text.Length > 0 && _audioDropdown.value != 0;
    }
}
