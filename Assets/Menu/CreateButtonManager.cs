using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using Unity.VisualScripting;

public class CreateButtonManager : MonoBehaviour
{
    [SerializeField] InputField _levelNameInput;
    [SerializeField] InputField _bpmInput;
    [SerializeField] Dropdown _audioDropdown;
    Button _createButton;
    void Start()
    {
        _createButton = GetComponent<Button>();
        _createButton.onClick.AddListener(CreateLevelOnClick);
    }

    void CreateLevelOnClick() {
        StartCoroutine(CreateLevel());
    }
    
    IEnumerator CreateLevel() {
        float.TryParse(_bpmInput.text.Replace(".", ","), out float bpm);
        Level level = new (_levelNameInput.text, null, bpm, "Audio/" + _audioDropdown.options[_audioDropdown.value].text);
        yield return StartCoroutine(LevelManager.CreateLevel(level));
        // level.SaveLevel();
        // LevelCompletionsManager.AddLevelCompletion(level.id);
        // LevelSettings.AddLevelSettings(level.id);
        // Debug.Log($"Creating level: {levelNameInput.text} with BPM: {bpmInput.text} and audio: {audioDropdown.options[audioDropdown.value].text}");
        SceneManager.LoadScene("LevelSelect");
    }

    public void VerifyInputs() {
        _createButton.interactable = _levelNameInput.text.Length > 0 && _bpmInput.text.Length > 0 && _audioDropdown.value != 0;
    }
}
