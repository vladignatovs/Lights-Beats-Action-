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
    bool _isFriendRequestSubscriptionActive;
    bool _isFriendshipSubscriptionActive;
    readonly Dictionary<System.Guid, UserFriendRequestState> _friendRequestStateByUserId = new();

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

        if (_isFriendRequestSubscriptionActive && SupabaseManager.Instance != null && SupabaseManager.Instance.FriendRequest != null) {
            SupabaseManager.Instance.FriendRequest.OnFriendRequestsChanged -= HandleFriendRequestsChanged;
        }
        _isFriendRequestSubscriptionActive = false;

        if (_isFriendshipSubscriptionActive && SupabaseManager.Instance != null && SupabaseManager.Instance.Friendship != null) {
            SupabaseManager.Instance.Friendship.OnFriendshipsChanged -= HandleFriendshipsChanged;
        }
        _isFriendshipSubscriptionActive = false;
    }

    async void HandleSectionChanged(ServerSection section) {
        if (section != ServerSection.User) return;

        if (!_isBlockSubscriptionActive && SupabaseManager.Instance != null && SupabaseManager.Instance.Block != null) {
            SupabaseManager.Instance.Block.OnBlocksChanged += HandleBlocksChanged;
            _isBlockSubscriptionActive = true;
        }

        if (!_isFriendRequestSubscriptionActive && SupabaseManager.Instance != null && SupabaseManager.Instance.FriendRequest != null) {
            SupabaseManager.Instance.FriendRequest.OnFriendRequestsChanged += HandleFriendRequestsChanged;
            _isFriendRequestSubscriptionActive = true;
        }

        if (!_isFriendshipSubscriptionActive && SupabaseManager.Instance != null && SupabaseManager.Instance.Friendship != null) {
            SupabaseManager.Instance.Friendship.OnFriendshipsChanged += HandleFriendshipsChanged;
            _isFriendshipSubscriptionActive = true;
        }

        await _paginationManager.GoToPage(0);
    }

    async void HandleBlocksChanged() {
        if (_serverSectionManager.CurrentSection != ServerSection.User) {
            return;
        }

        await _paginationManager.ReloadPage();
    }

    async void HandleFriendRequestsChanged() {
        if (_serverSectionManager.CurrentSection != ServerSection.User) {
            return;
        }

        await _paginationManager.ReloadPage();
    }

    async void HandleFriendshipsChanged() {
        if (_serverSectionManager.CurrentSection != ServerSection.User) {
            return;
        }

        await _paginationManager.ReloadPage();
    }

    async void OnPageLoaded(List<UserMetadata> metadatas) {
        var blockedUserIds = await SupabaseManager.Instance.Block.GetBlockedUserIds();
        var usersWhoBlockedMeIds = await SupabaseManager.Instance.Block.GetUsersWhoBlockedMeIds();
        var outgoing = await SupabaseManager.Instance.FriendRequest.GetOutgoingRequests();
        var incoming = await SupabaseManager.Instance.FriendRequest.GetIncomingRequests();
        var friendships = await SupabaseManager.Instance.Friendship.GetMyFriendships();
        foreach (var metadata in metadatas) {
            metadata.isBlocked = blockedUserIds.Contains(metadata.id);
            metadata.hasBlocked = usersWhoBlockedMeIds.Contains(metadata.id);

            if (!_friendRequestStateByUserId.TryGetValue(metadata.id, out var relationState)) {
                relationState = new UserFriendRequestState();
                _friendRequestStateByUserId[metadata.id] = relationState;
            }

            relationState.hasOutgoingRequest = outgoing.Any(x => x.ReceiverId == metadata.id && x.Accepted != true);
            relationState.hasIncomingRequest = incoming.Any(x => x.SenderId == metadata.id && x.Accepted == null);
            relationState.isFriend = friendships.Any(x => x.FriendId == metadata.id || x.FriendedId == metadata.id);
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

    public UserFriendRequestState GetFriendRequestState(System.Guid userId) {
        if (!_friendRequestStateByUserId.TryGetValue(userId, out var state)) {
            state = new UserFriendRequestState();
            _friendRequestStateByUserId[userId] = state;
        }

        return state;
    }

    public async Task<bool> OnToggleFriendRequest(System.Guid userId, bool hasOutgoingRequest) {
        if (hasOutgoingRequest) {
            var outgoingRequests = await SupabaseManager.Instance.FriendRequest.GetOutgoingRequests();
            var outgoing = outgoingRequests
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefault(x => x.ReceiverId == userId && x.Accepted != true);

            if (outgoing == null) {
                return false;
            }

            await SupabaseManager.Instance.FriendRequest.DeleteRequest(outgoing.Id);
            return false;
        }

        await SupabaseManager.Instance.FriendRequest.SendRequest(userId);
        return true;
    }

    public async Task<bool> OnRespondToFriendRequest(System.Guid userId, bool accept) {
        var incomingRequests = await SupabaseManager.Instance.FriendRequest.GetIncomingRequests();
        var incoming = incomingRequests
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefault(x => x.SenderId == userId && x.Accepted == null);

        if (incoming == null) {
            return false;
        }

        await SupabaseManager.Instance.FriendRequest.SetRequestAccepted(incoming.Id, accept);
        return false;
    }

    public async Task OnRemoveFriend(System.Guid userId) {
        await SupabaseManager.Instance.Friendship.DeleteFriendship(userId);
    }
}
