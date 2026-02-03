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
        await LocalLevelManager.CreateNewLevel(
            _levelNameInput.text,
            _bpmInput.text.FloatParse(),
            _audioDropdown.options[_audioDropdown.value].text
        );
        // reload the scene after creating a new level to update the list of levels
        await SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
    }

    public void VerifyInputs() {
        _createButton.interactable = _levelNameInput.text.Length > 0 && _bpmInput.text.Length > 0 && _audioDropdown.value != 0;
    }
}
