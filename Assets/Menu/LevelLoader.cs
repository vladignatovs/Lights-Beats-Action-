using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using TMPro;

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
    [SerializeField] Button _localPreviousButton;
    [SerializeField] Button _localNextButton;
    [SerializeField] TMP_InputField _localPageInput;
    [SerializeField] TMP_Text _localPageText;
    LocalLevelManager _localLevelManager;

    [Header ("ServerLevels")]
    [SerializeField] Transform _serverContentTransform;
    [SerializeField] GameObject _serverLevelCard;
    [SerializeField] Button _serverPreviousButton;
    [SerializeField] Button _serverNextButton;
    [SerializeField] TMP_InputField _serverPageInput;
    [SerializeField] TMP_Text _serverPageText;
    ServerLevelManager _serverLevelManager;

    [Header ("Other")]
    [SerializeField] MainMenuManager _mainMenuManager;
    [SerializeField] ConfirmationManager _confirmationManager;
    PaginationManager _paginationManager;

    private void Awake() {
        _officialLevelManager = new();
        _localLevelManager = new();
        _serverLevelManager = new();
        _paginationManager = new(
            _localLevelManager,
            _serverLevelManager
        );
    }

    void Start() => HandleStateChanged(StateNameManager.LatestMainMenuState);
    
    void OnEnable() {
        _mainMenuManager.OnStateChanged += HandleStateChanged;
        
        // Local level pagination
        _localPreviousButton.onClick.AddListener(OnLocalPreviousClicked);
        _localNextButton.onClick.AddListener(OnLocalNextClicked);
        _localPageInput.onEndEdit.AddListener(OnLocalPageInputChanged);
        
        // Server level pagination
        _serverPreviousButton.onClick.AddListener(OnServerPreviousClicked);
        _serverNextButton.onClick.AddListener(OnServerNextClicked);
        _serverPageInput.onEndEdit.AddListener(OnServerPageInputChanged);
    }
    
    void OnDisable() {
        _mainMenuManager.OnStateChanged -= HandleStateChanged;
        
        // Local level pagination
        _localPreviousButton.onClick.RemoveListener(OnLocalPreviousClicked);
        _localNextButton.onClick.RemoveListener(OnLocalNextClicked);
        _localPageInput.onEndEdit.RemoveListener(OnLocalPageInputChanged);
        
        // Server level pagination
        _serverPreviousButton.onClick.RemoveListener(OnServerPreviousClicked);
        _serverNextButton.onClick.RemoveListener(OnServerNextClicked);
        _serverPageInput.onEndEdit.RemoveListener(OnServerPageInputChanged);
    }

    async void HandleStateChanged(MainMenuState state) {
        _paginationManager.GoToPage(0); // TODO: try to preserve the last opened page
        await LoadAndRenderCurrentPage();
        
        // Update pagination UI for the current state
        if (state == MainMenuState.Local) {
            UpdateLocalPaginationUI();
        } else if (state == MainMenuState.Server) {
            UpdateServerPaginationUI();
        }
    }

    async void OnLocalNextClicked() {
        _paginationManager.GoToNextPage();
        await LoadAndRenderCurrentPage();
        UpdateLocalPaginationUI();
    }

    async void OnLocalPreviousClicked() {
        _paginationManager.GoToPreviousPage();
        await LoadAndRenderCurrentPage();
        UpdateLocalPaginationUI();
    }
    
    async void OnLocalPageInputChanged(string value) {
        if (!int.TryParse(value, out int pageNumber)) {
            // Invalid input, reset to current page
            UpdateLocalPaginationUI();
            return;
        }
        
        // Convert from 1-indexed (display) to 0-indexed (internal)
        int targetPage = pageNumber - 1;
        
        // Clamp to valid range
        int totalPages = _paginationManager.GetLocalTotalPages();
        targetPage = Math.Max(0, Math.Min(targetPage, totalPages - 1));
        
        // Go to the page
        _paginationManager.GoToPage(targetPage);
        await LoadAndRenderCurrentPage();
        UpdateLocalPaginationUI();
    }
    
    void UpdateLocalPaginationUI() {
        int totalPages = _paginationManager.GetLocalTotalPages();
        int currentPage = _paginationManager.CurrentPage + 1; // Display as 1-indexed
        
        _localPageInput.text = currentPage.ToString();
        _localPageText.text = totalPages.ToString();
        _localPreviousButton.interactable = _paginationManager.CurrentPage > 0;
        _localNextButton.interactable = _paginationManager.CurrentPage < totalPages - 1;
    }
    
    async void OnServerNextClicked() {
        _paginationManager.GoToNextPage();
        await LoadAndRenderCurrentPage();
        UpdateServerPaginationUI();
    }

    async void OnServerPreviousClicked() {
        _paginationManager.GoToPreviousPage();
        await LoadAndRenderCurrentPage();
        UpdateServerPaginationUI();
    }
    
    async void OnServerPageInputChanged(string value) {
        if (!int.TryParse(value, out int pageNumber)) {
            // Invalid input, reset to current page
            UpdateServerPaginationUI();
            return;
        }
        
        // Convert from 1-indexed (display) to 0-indexed (internal)
        int targetPage = pageNumber - 1;
        
        // Clamp to valid range
        int totalPages = _paginationManager.GetServerTotalPages();
        targetPage = Math.Max(0, Math.Min(targetPage, totalPages - 1));
        
        // Go to the page
        _paginationManager.GoToPage(targetPage);
        await LoadAndRenderCurrentPage();
        UpdateServerPaginationUI();
    }
    
    void UpdateServerPaginationUI() {
        int totalPages = _paginationManager.GetServerTotalPages();
        int currentPage = _paginationManager.CurrentPage + 1; // Display as 1-indexed
        
        _serverPageInput.text = currentPage.ToString();
        _serverPageText.text = totalPages.ToString();
        _serverPreviousButton.interactable = _paginationManager.CurrentPage > 0;
        _serverNextButton.interactable = _paginationManager.CurrentPage < totalPages - 1;
    }

    async Task LoadAndRenderCurrentPage() {
        List<LevelMetadata> metadatas;
        Transform contentTransform;
        GameObject cardPrefab;

        switch (StateNameManager.LatestMainMenuState) {
            case MainMenuState.Official:
                metadatas = await _officialLevelManager.LazyLoadLevels();
                contentTransform = _officialContentTransform;
                cardPrefab = _officialLevelCard;
                break;
                
            case MainMenuState.Local:
                metadatas = await _paginationManager.GetLocalLevelsPage();
                contentTransform = _localContentTransform;
                cardPrefab = _localLevelCard;
                break;
                
            case MainMenuState.Server:
                metadatas = await _paginationManager.GetServerLevelsPage();
                contentTransform = _serverContentTransform;
                cardPrefab = _serverLevelCard;
                break;
                
            default:
                return;
        }

        Clear(contentTransform);
        foreach (var metadata in metadatas) {
            var cardObject = Instantiate(cardPrefab, contentTransform);
            if (cardObject.TryGetComponent<ILevelCard>(out var card))
                card.Setup(metadata, this);
        }
    }

    // async Task LoadServerLevels() {
    //     Clear(_serverContentTransform);

    //     var metadatas = await _serverLevelManager.LazyLoadLevels();
    //     foreach (var metadata in metadatas) {
    //         var cardObject = Instantiate(_serverLevelCard, _serverContentTransform);
    //         if (cardObject.TryGetComponent<ILevelCard>(out var card))
    //             card.Setup(metadata, this);
    //     }
    // }

    async Task LoadOwnedServerLevels() {
        Clear(_ownedServerContentTransform);

        var metadatas = await _serverLevelManager.LazyLoadOwnedLevels();
        foreach (var metadata in metadatas) {
            var cardObject = Instantiate(_serverLevelCard, _ownedServerContentTransform);
            if (cardObject.TryGetComponent<ILevelCard>(out var card))
                card.Setup(metadata, this);
        }
    }

    // TODO: change this to a filter for fuzzy search
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
        Debug.Log("[LevelLoader]: " + metadata.audioPath);
        AudioClip audioClip = Resources.Load<AudioClip>("Audio/" + metadata.audioPath);
        StateNameManager.LoadedAudioClip = audioClip;

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
            Debug.Log("[LevelLoader]" + level.serverId);
            level = await ServerLevelManager.PublishLevel(level);
            await LocalLevelManager.SaveLevel(level); // save the level as publishlevel might strip the serverid
        });
    }

    public void OnDeleteLocalLevel(int id) {
        _confirmationManager.ShowConfirmation(
            async () => { 
                LocalLevelManager.DeleteLevel(id); 
                _mainMenuManager.ToLocal();
                await ReloadScene();
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
        await ReloadScene();
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
                _mainMenuManager.ToServer();
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
