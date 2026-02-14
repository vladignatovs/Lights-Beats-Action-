using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using System.Linq;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour, ILevelCardCallbacks {
    [Header ("OfficialLevels")]
    [SerializeField] RectTransform _officialContentTransform;
    [SerializeField] GameObject _officialLevelCard;
    OfficialLevelManager _officialLevelManager;

    [Header ("LocalLevels")]
    [SerializeField] Transform _localContentTransform;
    [SerializeField] GameObject _localLevelCard;
    [SerializeField] LevelPaginationManager _localPaginationManager;
    LocalLevelManager _localLevelManager;

    [Header ("ServerLevels")]
    [SerializeField] Transform _serverContentTransform;
    [SerializeField] GameObject _serverLevelCard;
    [SerializeField] LevelPaginationManager _serverPaginationManager;
    [SerializeField] ServerLevelFilterPanel _serverFilterPanel;
    ServerLevelManager _serverLevelManager;

    [Header ("Other")]
    [SerializeField] MainMenuManager _mainMenuManager;
    [SerializeField] ConfirmationManager _confirmationManager;

    private void Awake() {
        _officialLevelManager = new();
        _localLevelManager = new();
        _serverLevelManager = new();

        // Provide the pagination managers with a page provider
        _localPaginationManager.Initialize(_localLevelManager);
        _serverPaginationManager.Initialize(_serverLevelManager);

        // Initialize filter panel with pagination manager
        _serverFilterPanel.Initialize(_serverPaginationManager);
    }

    async void Start() {
        // On start, load persisted page
        int pageToLoad = StateNameManager.LastLevelPage;
        // TODO: might want to try and preserve the opened page on reload of the scene?
        StateNameManager.LastLevelPage = 0; // clear persisted to account for reloading to show 0 page
        await LoadCurrentState(pageToLoad);
    }
    
    void OnEnable() {
        _mainMenuManager.OnStateChanged += HandleStateChanged;
        
        // Subscribe to pagination events
        _localPaginationManager.OnPageLoaded += OnLocalPageLoaded;
        _serverPaginationManager.OnPageLoaded += OnServerPageLoaded;
    }
    
    void OnDisable() {
        _mainMenuManager.OnStateChanged -= HandleStateChanged;
        
        // Unsubscribe from pagination events
        _localPaginationManager.OnPageLoaded -= OnLocalPageLoaded;
        _serverPaginationManager.OnPageLoaded -= OnServerPageLoaded;
    }

    async void HandleStateChanged(MainMenuState state) {
        await LoadCurrentState(0);
    }
    
    async Task LoadCurrentState(int pageNumber) {
        switch (StateNameManager.LatestMainMenuState) {
            case MainMenuState.Official:
                await LoadOfficialLevels();
                break;
            case MainMenuState.Local:
                await _localPaginationManager.GoToPage(pageNumber);
                break;
            case MainMenuState.Server:
                await _serverPaginationManager.GoToPage(pageNumber);
                break;
        }
    }

    void OnLocalPageLoaded(List<LevelMetadata> metadatas) {
        RenderLevelCards(metadatas, _localLevelCard, _localContentTransform);
    }

    void OnServerPageLoaded(List<LevelMetadata> metadatas) {
        RenderLevelCards(metadatas, _serverLevelCard, _serverContentTransform);
    }

    async Task LoadOfficialLevels() {
        var metadatas = await _officialLevelManager.LazyLoadLevels();
        RenderLevelCards(metadatas, _officialLevelCard, _officialContentTransform);
    }

    void RenderLevelCards(List<LevelMetadata> metadatas, GameObject cardPrefab, Transform contentTransform) {
        // clear the content of previously loaded children
        for (int i = contentTransform.childCount - 1; i >= 0; i--) {
            Destroy(contentTransform.GetChild(i).gameObject);
        }
        // load in the newly fetched ones
        foreach (var metadata in metadatas) {
            var cardObject = Instantiate(cardPrefab, contentTransform);
            if (cardObject.TryGetComponent<ILevelCard>(out var card))
                card.Setup(metadata, this);
        }
    }

    [UsedImplicitly]
    public async void ImportLevel() {
        await LocalLevelManager.ImportLevel();
        await _localPaginationManager.ReloadPage();
    }

#region Level Card Callbacks
    public async Task OnPlayLevel(LevelMetadata metadata) {
        // Save the current page before leaving to play level
        switch (StateNameManager.LatestMainMenuState) {
            case MainMenuState.Local:
                _localPaginationManager.PersistCurrentPage();
                break;
            case MainMenuState.Server:
                _serverPaginationManager.PersistCurrentPage();
                break;
        }
        
        try {
            await _mainMenuManager.ToGame();
        }
        catch (Exception e) {
            Debug.Log(e);
        }

        // Initialize all static level data
        LevelInitializer.InitializeLevel(
            await (StateNameManager.LatestMainMenuState switch {
                MainMenuState.Official => _officialLevelManager.LoadLevel(metadata.id),
                MainMenuState.Local => _localLevelManager.LoadLevel(metadata.id),
                MainMenuState.Server => _serverLevelManager.LoadLevel(metadata.serverId.Value),
                _ => null
            })
        );

        // start loading the scene AFTER THE AUDIO CLIP and LEVEL, as it might start the mono behaviour scripts beforehand
        await SceneStateManager.LoadGame();
    }

    public async Task OnOpenEditor(LevelMetadata metadata) {
        // load the level and initialize it before going to the editor
        Level level = await _localLevelManager.LoadLevel(metadata.id);
        LevelInitializer.InitializeLevel(level);
        await SceneStateManager.LoadEditor();
    }

    public async Task OnExportLevel(int id) {
        await LocalLevelManager.ExportLevel(id);
    }

    public void OnPublishLevel(int id) {
        _confirmationManager.ShowConfirmation(async () => {
            var level = await _localLevelManager.LoadLevel(id);
            level = await ServerLevelManager.PublishLevel(level);
            await LocalLevelManager.SaveLevel(level); // save the level as publishlevel might strip the serverid
            await _serverPaginationManager.ReloadPage();
        });
    }

    public void OnDeleteLocalLevel(int id) {
        _confirmationManager.ShowConfirmation(
            async () => { 
                LocalLevelManager.DeleteLevel(id); 
                await _localPaginationManager.ReloadPage();
            });
    }

    public async Task OnUpdateLevelName(int id, string value) {
        if (string.IsNullOrWhiteSpace(value)) return;

        var level = await _localLevelManager.LoadLevel(id);
        if (level == null || level.name == value) return;
        level.name = value;
        await LocalLevelManager.SaveLevel(level);
    }

    public async Task OnUpdateLevelAudioPath(int id, string value) {
        if (string.IsNullOrWhiteSpace(value)) return;

        var level = await _localLevelManager.LoadLevel(id);
        if (level == null || level.audioPath == value) return;
        level.audioPath = value;
        await LocalLevelManager.SaveLevel(level);
        await _localPaginationManager.ReloadPage();
    }

    public async Task OnUpdateLevelBpm(int id, string value) {
        if (!value.FloatTryParse(out var bpm)) return;

        var level = await _localLevelManager.LoadLevel(id);
        // instead of comparing bpms directly we compare their differences to account for inconsistencies
        if (level == null || Math.Abs(level.bpm - bpm) < 0.001f) return;

        level.bpm = bpm;
        await LocalLevelManager.SaveLevel(level);
    }

    public void OnDeleteServerLevel(Guid id) {
        _confirmationManager.ShowConfirmation(
            async () => {
                await _serverLevelManager.DeleteLevel(id);
                await _serverPaginationManager.ReloadPage();
            }
        );
    }

    public async Task OnImportLevel(Guid id) {
        await _serverLevelManager.ImportLevel(id);
        _mainMenuManager.ToLocal();
        await SceneStateManager.Reload();
    }
#endregion
}
