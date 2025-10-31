using System.Collections;
using UnityEngine;
using LitJson;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Threading.Tasks;

public class LevelLoader : MonoBehaviour {
    [SerializeField] GameObject _goToButton;
    [SerializeField] Transform _contentTransform;
    [SerializeField] GameObject _createLevelPanel;

    async void Start() {
        await LevelManager.Initialize();
        foreach (var level in LevelManager.Levels) {
            Button levelButton = Instantiate(_goToButton, _contentTransform).GetComponent<Button>();
            levelButton.onClick.AddListener(() => LoadSceneAsync(level));
            levelButton.GetComponentInChildren<Text>().text = level.name;
        }
    }

    async void LoadSceneAsync(Level level) {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Level");
        asyncLoad.allowSceneActivation = false;

        // Load the audio clip
        AudioClip audioClip = Resources.Load<AudioClip>(level.audioPath);
        StateNameManager.LoadedAudioClip = audioClip;

        StateNameManager.Level = await LevelManager.LoadLevel(level.localId);
        // Wait until the scene is fully loaded
        while (asyncLoad.progress < 0.9f) {
            await Task.Yield();
        }

        asyncLoad.allowSceneActivation = true;
    }

    public void showPanel() {
        _createLevelPanel.SetActive(!_createLevelPanel.activeSelf);
    }
}
