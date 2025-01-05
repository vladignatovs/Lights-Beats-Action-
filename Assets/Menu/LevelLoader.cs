using System.Collections;
using UnityEngine;
using LitJson;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class LevelLoader : MonoBehaviour {
    [SerializeField] GameObject _goToButton;
    [SerializeField] Transform _contentTransform;
    [SerializeField] GameObject _createLevelPanel;

    void Start() {
        StartCoroutine(LevelManager.GetLevels((List<Level> levels) => {
            if (levels != null) {
                foreach (Level level in levels) {
                    Button levelButton = Instantiate(_goToButton, _contentTransform).GetComponent<Button>();
                    // if(LevelCompletionsManager.GetCompletionFromId(level.id)) {
                    //     levelButton.GetComponent<Image>().color = Color.green;
                    // }
                    levelButton.onClick.AddListener(() => CustomOnClick(level.id, level.audioPath));
                    levelButton.GetComponentInChildren<Text>().text = level.levelName;
                }
            } else {
                Debug.LogError("Failed to load levels.");
            }
        }));
        // string path = Application.dataPath + "/Levels.json";
        // JsonData jsonData = JsonMapper.ToObject(File.ReadAllText(path));
        // foreach(JsonData level in jsonData["Levels"]) {
        //     Level _level = new() {
        //         id = (int)level["id"],
        //         levelName = (string)level["levelName"],
        //         audioPath = (string)level["audioPath"]
        //     };

        //     Button levelButton = Instantiate(_goToButton, _contentTransform).GetComponent<Button>();

        //     //TODO: actually interesting way of showing beaten levels.
        //     if(LevelCompletionsManager.GetCompletionFromId(_level.id)) {
        //         levelButton.GetComponent<Image>().color = Color.green;
        //     }

        //     levelButton.onClick.AddListener(() => CustomOnClick(_level.id, _level.audioPath));
        //     levelButton.GetComponentInChildren<Text>().text = _level.levelName;
        // }
    }

    void CustomOnClick(int id, string audioPath) {
        StartCoroutine(LoadSceneAsync(id, audioPath));
    }

    // sketchy, perhaps up to change 
    private IEnumerator LoadSceneAsync(int id, string audioPath) {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Level");
        asyncLoad.allowSceneActivation = false;

        // Load the audio clip
        AudioClip audioClip = Resources.Load<AudioClip>(audioPath);
        StateNameManager.Level.id = id; // needed in order to get level by id further
        StateNameManager.LoadedAudioClip = audioClip;
        yield return StartCoroutine(LevelManager.GetLevelById((Level level) => {
            StateNameManager.Level = level;
        }));
        // Wait until the scene is fully loaded
        while (!asyncLoad.isDone) {
            if (asyncLoad.progress >= 0.9f) {
                asyncLoad.allowSceneActivation = true;
            }
            yield return null;
        }
    }

    public void showPanel() {
        _createLevelPanel.SetActive(!_createLevelPanel.activeSelf);
    }
}
