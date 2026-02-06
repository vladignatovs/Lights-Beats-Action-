using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System;

public class LevelLoader : MonoBehaviour, ILevelCardCallbacks {
    [Header ("OfficialLevels")]
    [SerializeField] RectTransform _officialContentTransform;
    [SerializeField] GameObject _officialLevelCard;
    OfficialLevelManager _officialLevelManager;

    [Header ("LocalLevels")]
    [SerializeField] Transform _localContentTransform;
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
    [SerializeField] ConfirmationManager _confirmationManager;

    private void Awake() {
        _officialLevelManager = new();
        _localLevelManager = new();
        _serverLevelManager = new();
    }

    void Start() => HandleStateChanged(StateNameManager.LatestMainMenuState);
    void OnEnable() => _mainMenuManager.OnStateChanged += HandleStateChanged;
    void OnDisable() => _mainMenuManager.OnStateChanged -= HandleStateChanged;

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

        var metadatas = await _officialLevelManager.LazyLoadLevels();
        foreach(var metadata in metadatas) {
            var cardObject = Instantiate(_officialLevelCard, _officialContentTransform);
            if (cardObject.TryGetComponent<ILevelCard>(out var card))
                card.Setup(metadata, this);
        }
    }

    async Task LoadLocalLevels() {
        Clear(_localContentTransform);

        var metadatas = await _localLevelManager.LazyLoadLevels();
        foreach (var metadata in metadatas) {
            var cardObject = Instantiate(_localLevelCard, _localContentTransform);
            if (cardObject.TryGetComponent<ILevelCard>(out var card))
                card.Setup(metadata, this);
        }
    }

    async Task LoadServerLevels() {
        Clear(_serverContentTransform);

        var metadatas = await _serverLevelManager.LazyLoadLevels();
        foreach (var metadata in metadatas) {
            var cardObject = Instantiate(_serverLevelCard, _serverContentTransform);
            if (cardObject.TryGetComponent<ILevelCard>(out var card))
                card.Setup(metadata, this);
        }
    }

    async Task LoadOwnedServerLevels() {
        Clear(_ownedServerContentTransform);

        var metadatas = await _serverLevelManager.LazyLoadOwnedLevels();
        foreach (var metadata in metadatas) {
            var cardObject = Instantiate(_serverLevelCard, _ownedServerContentTransform);
            if (cardObject.TryGetComponent<ILevelCard>(out var card))
                card.Setup(metadata, this);
        }
    }

    public async void ToggleOwnedServerPanel() {
        var state = !_ownedServerLevelsPanel.activeSelf;
        if(state) 
            await LoadOwnedServerLevels();
        _ownedServerLevelsPanel.SetActive(state);
        Overlay.ToggleOverlay(state);
    }

    public async void ImportLevel() {
        await LocalLevelManager.ImportLevel();
        await ReloadScene();
    }

    public async Task OnPlayLevel(LevelMetadata metadata) {
        try {
            await _mainMenuManager.ToGame();
        }
        catch (Exception e) {
            Debug.Log(e);
        }

        // Load the audio clip
        AudioClip audioClip = Resources.Load<AudioClip>("Audio/" + metadata.audioPath);
        StateNameManager.LoadedAudioClip = audioClip;

        // TODO: MAKE OWNED SERVER LEVELS PLAYABLE AS THE GAME TRIES TO LOAD THEM AS LOCAL BECAUSE OF THE STATE
        StateNameManager.Level = StateNameManager.LatestMainMenuState switch {
            MainMenuState.Official => await _officialLevelManager.LoadLevel(metadata.id),
            MainMenuState.Local => await _localLevelManager.LoadLevel(metadata.id),
            MainMenuState.Server => await _serverLevelManager.LoadLevel(metadata.serverId.Value),
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

    public async Task OnExportLevel(int id) {
        await LocalLevelManager.ExportLevel(id);
    }

    public void OnPublishLevel(int id) {
        _confirmationManager.ShowConfirmation(async () => {
            var level = await _localLevelManager.LoadLevel(id);
            await ServerLevelManager.PublishLevel(level);
        });
    }

    public void OnDeleteLocalLevel(int id) {
        _confirmationManager.ShowConfirmation(
            async () => { 
                LocalLevelManager.DeleteLevel(id); 
                await ReloadScene();
            });
    }

    public void OnDeleteServerLevel(Guid id) {
        _confirmationManager.ShowConfirmation(
            async () => {
                await _serverLevelManager.DeleteLevel(id);
                await ReloadScene();
            }
        );
    }

    public async Task OnImportLevel(Guid id) {
        await _serverLevelManager.ImportLevel(id);
        _mainMenuManager.ToLocal();
        await ReloadScene();
    }

    async Task ReloadScene() {
        await SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
    }

    void Clear(Transform transform) {
        for (int i = transform.childCount - 1; i >= 0; i--) {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}
