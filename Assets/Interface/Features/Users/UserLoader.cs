using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class UserLoader : MonoBehaviour, IUserCardCallbacks {
    [SerializeField] Transform _contentTransform;
    [SerializeField] GameObject _userCard;
    [SerializeField] UserPaginationManager _paginationManager;
    [SerializeField] UserFilterPanel _filterPanel;
    [SerializeField] ServerSectionManager _serverSectionManager;

    UserPageManager _userPageManager;
    bool _isBlockSubscriptionActive;

    void Awake() {
        _userPageManager = new();
        _paginationManager.Initialize(_userPageManager);
        _filterPanel.Initialize(_paginationManager);
    }

    void OnEnable() {
        _paginationManager.OnPageLoaded += OnPageLoaded;
        _serverSectionManager.OnSectionChanged += HandleSectionChanged;

        if (_serverSectionManager.CurrentSection == ServerSection.User) {
            HandleSectionChanged(ServerSection.User);
        }
    }

    void OnDisable() {
        _paginationManager.OnPageLoaded -= OnPageLoaded;
        _serverSectionManager.OnSectionChanged -= HandleSectionChanged;

        if (_isBlockSubscriptionActive && SupabaseManager.Instance != null && SupabaseManager.Instance.Block != null) {
            SupabaseManager.Instance.Block.OnBlocksChanged -= HandleBlocksChanged;
        }
        _isBlockSubscriptionActive = false;
    }

    async void HandleSectionChanged(ServerSection section) {
        if (section != ServerSection.User) return;

        if (!_isBlockSubscriptionActive && SupabaseManager.Instance != null && SupabaseManager.Instance.Block != null) {
            SupabaseManager.Instance.Block.OnBlocksChanged += HandleBlocksChanged;
            _isBlockSubscriptionActive = true;
        }

        await _paginationManager.GoToPage(0);
    }

    async void HandleBlocksChanged() {
        if (_serverSectionManager.CurrentSection != ServerSection.User) {
            return;
        }

        await _paginationManager.ReloadPage();
    }

    async void OnPageLoaded(List<UserMetadata> metadatas) {
        var blockedUserIds = await SupabaseManager.Instance.Block.GetBlockedUserIds();
        foreach (var metadata in metadatas) {
            metadata.isBlocked = blockedUserIds.Contains(metadata.id);
        }

        RenderUserCards(metadatas);
    }

    void RenderUserCards(List<UserMetadata> metadatas) {
        for (int i = _contentTransform.childCount - 1; i >= 0; i--) {
            Destroy(_contentTransform.GetChild(i).gameObject);
        }

        foreach (var metadata in metadatas) {
            var cardObject = Instantiate(_userCard, _contentTransform);
            if (!cardObject.TryGetComponent<IUserCard>(out var card)) continue;
            card.Setup(metadata, this);
        }
    }

    public async Task<bool> OnToggleBlockUser(System.Guid userId, bool isBlocked) {
        if (isBlocked) {
            await SupabaseManager.Instance.Block.UnblockUser(userId);
            return false;
        }

        await SupabaseManager.Instance.Block.BlockUser(userId);
        return true;
    }
}
