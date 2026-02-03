using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using TMPro;
using System;

public class LevelLoader : MonoBehaviour {
    [Header ("OfficialLevels")]
    [SerializeField] RectTransform _officialContentTransform;
    [SerializeField] GameObject _officialLevelCard;
    OfficialLevelManager _officialLevelManager;

    [Header ("LocalLevels")]
    [SerializeField] Transform _localContentTransform;
    [SerializeField] GameObject _createLevelPanel;
    [SerializeField] GameObject _localLevelCard;
    [SerializeField] GameObject _ownedServerLevelsPanel;
    [SerializeField] Transform _ownedServerContentTransform;
    LocalLevelManager _localLevelManager;

    [Header ("ServerLevels")]
    [SerializeField] Transform _serverContentTransform;
    [SerializeField] GameObject _serverLevelCard;
    ServerLevelManager _serverLevelManager;

    [Header ("Other")]
    [SerializeField] MainMenuManager _mainMenuManager;

    private void Awake() {
        _officialLevelManager = new();
        _localLevelManager = new();
        _serverLevelManager = new();
    }

    void Start() {
        // handle initial state which is needed after a player exits a level
        HandleStateChanged(StateNameManager.LatestMainMenuState);
    }

    void OnEnable() {
        _mainMenuManager.OnStateChanged += HandleStateChanged;
    }

    void OnDisable() {
        _mainMenuManager.OnStateChanged -= HandleStateChanged;
    }


    void HandleStateChanged(MainMenuState state) {
        switch (state) {
            case MainMenuState.Official:
                _ = LoadOfficialLevels();
                break;
            case MainMenuState.Local:
                _ = LoadLocalLevels();
                break;
            case MainMenuState.Server:
                _ = LoadServerLevels();
                break;
        }
    }

    async Task LoadOfficialLevels() {
        Clear(_officialContentTransform);

        var levelMetas = await _officialLevelManager.LazyLoadLevels();
        foreach(var levelMeta in levelMetas) {
            Button levelButton = Instantiate(_officialLevelCard, _officialContentTransform).GetComponent<Button>();
            levelButton.onClick.AddListener(async () => await LoadSceneAsync(levelMeta));
            levelButton.GetComponentInChildren<TMP_Text>().text = levelMeta.name;
        }
    }

    async Task LoadLocalLevels() {
        Clear(_localContentTransform);

        var levelMetas = await _localLevelManager.LazyLoadLevels();
        foreach (var levelMeta in levelMetas) {
            Button levelButton = Instantiate(_localLevelCard, _localContentTransform).GetComponent<Button>();
            // put each new level at the top
            levelButton.onClick.AddListener(async () => await LoadSceneAsync(levelMeta));
            // setting the information about the level
            var parent = levelButton.transform;
            SetChildText(parent, "Name", levelMeta.name);
            var button = parent.Find("ExportButton").GetComponent<Button>();
            button.onClick.AddListener(async() => await ExportLevel(levelMeta.id));
        }
    }

    // TODO: apply styles to goTo buttons evenly
    async Task LoadServerLevels() {
        Clear(_serverContentTransform);

        var levelMetas = await _serverLevelManager.LazyLoadLevels();
        foreach (var levelMeta in levelMetas) {
            Button levelButton = Instantiate(_serverLevelCard, _serverContentTransform).GetComponent<Button>();
            levelButton.onClick.AddListener(async () => await LoadSceneAsync(levelMeta));
            // setting the information about the level
            var parent = levelButton.transform;
            SetChildText(parent, "Name", levelMeta.name);
            var creatorButton = parent.Find("CreatorButton");
            SetChildText(creatorButton, "CreatorUsername", levelMeta.creatorUsername);
            SetChildText(parent, "Bpm", levelMeta.bpm.ToString());
            SetChildText(parent, "AudioName", levelMeta.audioPath);
        }
    }

    async Task LoadOwnedServerLevels() {
        Clear(_ownedServerContentTransform);

        var levelMetas = await _serverLevelManager.LazyLoadOwnedLevels();
        foreach (var levelMeta in levelMetas) {
            Button levelButton = Instantiate(_serverLevelCard, _ownedServerContentTransform).GetComponent<Button>();
            var parent = levelButton.transform;
            SetChildText(parent, "Name", levelMeta.name);
            var creatorButton = parent.Find("CreatorButton");
            SetChildText(creatorButton, "CreatorUsername", levelMeta.creatorUsername);
            SetChildText(parent, "Bpm", levelMeta.bpm.ToString());
            SetChildText(parent, "AudioName", levelMeta.audioPath);
        }
    }

    void SetChildText(Transform parent, string childName, string text) {
        Transform child = parent.Find(childName);
        if (child != null) {
            TMP_Text textComponent = child.GetComponent<TMP_Text>();
            if (textComponent != null) {
                textComponent.text = text;
            }
        }
    }

    async Task LoadSceneAsync(LevelMetadata levelMeta) {
        try {
            await _mainMenuManager.ToGame();
        }
        catch (Exception e) {
            Debug.Log(e);
        }

        // Load the audio clip
        AudioClip audioClip = Resources.Load<AudioClip>("Audio/" + levelMeta.audioPath);
        StateNameManager.LoadedAudioClip = audioClip;

        StateNameManager.Level = StateNameManager.LatestMainMenuState switch {
            MainMenuState.Official => await _officialLevelManager.LoadLevel(levelMeta.id),
            MainMenuState.Local => await _localLevelManager.LoadLevel(levelMeta.id),
            MainMenuState.Server => await _serverLevelManager.LoadLevel((Guid)levelMeta.serverId),
            _ => null
        };

        // start loading the scene AFTER THE AUDIO CLIP and LEVEL, as it might start the mono behaviour scripts beforehand
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Level");
        asyncLoad.allowSceneActivation = false;

        // Wait until the scene is fully loaded
        while (asyncLoad.progress < 0.9f) {
            await Task.Yield();
        }

        asyncLoad.allowSceneActivation = true;
    }

    public void ToggleCreateLevelPanel() {
        _createLevelPanel.SetActive(!_createLevelPanel.activeSelf);
    }

    public async void ToggleOwnedServerPanel() {
        var state = !_ownedServerLevelsPanel.activeSelf;
        if(state) 
            await LoadOwnedServerLevels();
        _ownedServerLevelsPanel.SetActive(state);
    }

    public async Task ExportLevel(int id) {
        await _localLevelManager.ExportLevel(id);
    }

    public async void ImportLevel() {
        await _localLevelManager.ImportLevel();
        // reload the scene to show the newly imported level
        await SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
    }

    void Clear(Transform transform) {
        for (int i = transform.childCount - 1; i >= 0; i--) {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}
