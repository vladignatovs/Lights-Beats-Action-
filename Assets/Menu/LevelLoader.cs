using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using TMPro;

public class LevelLoader : MonoBehaviour {
    [SerializeField] GameObject _goToButton;
    [SerializeField] Transform _contentTransform;
    [SerializeField] GameObject _createLevelPanel;
    [SerializeField] MainMenuManager _mainMenuManager;

    // should fetch official levels on start instead of local
    async void Start() {
        await LevelManager.Initialize();
        foreach (var level in LevelManager.Levels) {
            Button levelButton = Instantiate(_goToButton, _contentTransform).GetComponent<Button>();
            levelButton.onClick.AddListener(async () => await LoadSceneAsync(level));
            levelButton.GetComponentInChildren<TMP_Text>().text = level.name;
        }
    }

    async Task LoadSceneAsync(Level level) {
        try {
            await _mainMenuManager.ToGame();
        }
        catch (System.Exception e) {
            Debug.Log(e);
        }
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

    public void TogglePanel() {
        _createLevelPanel.SetActive(!_createLevelPanel.activeSelf);
    }
}
