using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

public class UserAccountManager : MonoBehaviour, IUserCardCallbacks {
    [SerializeField] GameObject _accountPanel;
    [SerializeField] ConfirmationManager _confirmationManager;
    [SerializeField] TMP_Text _usernameText;
    [SerializeField] TMP_InputField _changeUsernameInput;
    [SerializeField] TMP_InputField _changeEmailInput;
    [SerializeField] TMP_InputField _changePasswordInput;
    [SerializeField] TMP_Text _accountActionText;
    [SerializeField] Transform _blockedUsersContent;
    [SerializeField] GameObject _blockedUserCardPrefab;
    [SerializeField] Transform _sentRequestsContent;
    [SerializeField] GameObject _sentRequestUserCardPrefab;
    [SerializeField] Transform _receivedRequestsContent;
    [SerializeField] GameObject _receivedRequestUserCardPrefab;
    [SerializeField] Transform _friendsContent;
    [SerializeField] GameObject _friendUserCardPrefab;

    readonly Dictionary<System.Guid, UserFriendRequestState> _friendRequestStateByUserId = new();

    void Start() {
        // set immediately from cache
        _usernameText.SetText(SupabaseManager.Instance.User.Name);
        // subscribe for updates
        SupabaseManager.Instance.User.OnNameChanged += HandleNameChanged;
    }

    void OnDestroy() {
        SupabaseManager.Instance.User.OnNameChanged -= HandleNameChanged;
    }

    void HandleNameChanged(string name) {
        _usernameText.SetText(name);
    }

    public async void ToggleAccountPanel() {
        bool shouldOpen = !_accountPanel.activeSelf;
        Overlay.ToggleOverlay(shouldOpen);
        _accountPanel.SetActive(shouldOpen);

        if (shouldOpen) {
            RefreshAccountInputs();
            await RefreshBlockedUsers();
            await RefreshFriendRequests();
            await RefreshFriends();
        }
    }

    [UsedImplicitly]
    public void UpdateUsername() {
        _confirmationManager.ShowConfirmation(async () => await ConfirmUpdateUsername());
    }

    [UsedImplicitly]
    public void UpdateEmail() {
        _confirmationManager.ShowConfirmation(async () => await ConfirmUpdateEmail());
    }

    [UsedImplicitly]
    public void UpdatePassword() {
        _confirmationManager.ShowConfirmation(async () => await ConfirmUpdatePassword());
    }

    [UsedImplicitly]
    public void DeleteAccount() {
        _confirmationManager.ShowConfirmation(ConfirmDeleteAccount);
    }

    async Task RefreshFriendRequests() {
        _friendRequestStateByUserId.Clear();
        await RefreshSentRequests();
        await RefreshReceivedRequests();
    }

    async Task RefreshFriends() {
        for (int i = _friendsContent.childCount - 1; i >= 0; i--) {
            Destroy(_friendsContent.GetChild(i).gameObject);
        }

        var friendships = await SupabaseManager.Instance.Friendship.GetMyFriendships();
        if (friendships.Count == 0) {
            return;
        }

        if (!System.Guid.TryParse(SupabaseManager.Instance.Client.Auth.CurrentUser?.Id, out var me)) {
            return;
        }

        var friendIds = friendships
            .Select(x => x.FriendId == me ? x.FriendedId : x.FriendId)
            .Distinct()
            .ToList();

        foreach (var friendId in friendIds) {
            var user = await SupabaseManager.Instance.User.LoadUserById(friendId);
            if (user == null) continue;

            if (!_friendRequestStateByUserId.TryGetValue(user.id, out var state)) {
                state = new UserFriendRequestState();
                _friendRequestStateByUserId[user.id] = state;
            }
            state.isFriend = true;
            state.hasIncomingRequest = false;
            state.hasOutgoingRequest = false;

            var cardObject = Instantiate(_friendUserCardPrefab, _friendsContent);
            if (!cardObject.TryGetComponent<IUserCard>(out var card)) continue;
            card.Setup(user, this);
        }
    }

    public async void SignOut() {
        if (_accountPanel.activeSelf) {
            ToggleAccountPanel();
        }

        await SupabaseManager.Instance.Auth.SignOut();
    }

    void RefreshAccountInputs() {
        _changeUsernameInput.text = SupabaseManager.Instance.User.Name;
        _changeEmailInput.text = SupabaseManager.Instance.Auth.Email;
        _changePasswordInput.text = string.Empty;
        _accountActionText.text = string.Empty;
    }

    async Task ConfirmUpdateUsername() {
        try {
            await SupabaseManager.Instance.User.UpdateUsername(_changeUsernameInput.text);
            _changeUsernameInput.text = SupabaseManager.Instance.User.Name;
            _accountActionText.text = "Username updated.";
        } catch (Exception e) {
            _accountActionText.text = GetErrorMessage("Username update failed", e);
        }
    }

    async Task ConfirmUpdateEmail() {
        try {
            await SupabaseManager.Instance.Auth.UpdateEmail(_changeEmailInput.text);
            _accountActionText.text = "Email confirmation has been sent to you.";
        } catch (Exception e) {
            _accountActionText.text = GetErrorMessage("Email update failed", e);
        }
    }

    async Task ConfirmUpdatePassword() {
        try {
            await SupabaseManager.Instance.Auth.UpdatePassword(_changePasswordInput.text);
            _changePasswordInput.text = string.Empty;
            _accountActionText.text = "Password updated.";
        } catch (Exception e) {
            _accountActionText.text = GetErrorMessage("Password update failed", e);
        }
    }

    async void ConfirmDeleteAccount() {
        try {
            await SupabaseManager.Instance.User.DeleteCurrentUser();
            if (_accountPanel.activeSelf) {
                ToggleAccountPanel();
            }

            await SupabaseManager.Instance.Auth.SignOut();
        } catch (Exception e) {
            _accountActionText.text = GetErrorMessage("Account deletion failed", e);
        }
    }

    static string GetErrorMessage(string prefix, Exception e) {
        try {
            return prefix + ": " + JsonUtility.FromJson<AuthError>(e.Message).msg;
        } catch {
            return prefix + ".";
        }
    }

    async Task RefreshBlockedUsers() {
        for (int i = _blockedUsersContent.childCount - 1; i >= 0; i--) {
            Destroy(_blockedUsersContent.GetChild(i).gameObject);
        }

        var blockedIds = await SupabaseManager.Instance.Block.GetBlockedUserIds();
        if (blockedIds.Count == 0) {
            return;
        }

        var loadTasks = new List<Task<UserMetadata>>();
        foreach (var blockedId in blockedIds) {
            loadTasks.Add(SupabaseManager.Instance.User.LoadUserById(blockedId));
        }

        var blockedUsers = await Task.WhenAll(loadTasks);
        foreach (var user in blockedUsers.Where(x => x != null)) {
            user.isBlocked = true;
            var cardObject = Instantiate(_blockedUserCardPrefab, _blockedUsersContent);
            if (!cardObject.TryGetComponent<IUserCard>(out var card)) continue;
            card.Setup(user, this);
        }
    }

    async Task RefreshSentRequests() {
        for (int i = _sentRequestsContent.childCount - 1; i >= 0; i--) {
            Destroy(_sentRequestsContent.GetChild(i).gameObject);
        }

        var outgoing = await SupabaseManager.Instance.FriendRequest.GetOutgoingRequests();
        var pendingOrDenied = outgoing
            .Where(x => x.Accepted != true)
            .OrderByDescending(x => x.CreatedAt)
            .GroupBy(x => x.ReceiverId)
            .Select(x => x.First())
            .ToList();

        foreach (var request in pendingOrDenied) {
            var user = await SupabaseManager.Instance.User.LoadUserById(request.ReceiverId);
            if (user == null) continue;

            if (!_friendRequestStateByUserId.TryGetValue(user.id, out var state)) {
                state = new UserFriendRequestState();
                _friendRequestStateByUserId[user.id] = state;
            }
            state.hasOutgoingRequest = true;
            state.isFriend = false;

            var cardObject = Instantiate(_sentRequestUserCardPrefab, _sentRequestsContent);
            if (!cardObject.TryGetComponent<IUserCard>(out var card)) continue;
            card.Setup(user, this);
        }
    }

    async Task RefreshReceivedRequests() {
        for (int i = _receivedRequestsContent.childCount - 1; i >= 0; i--) {
            Destroy(_receivedRequestsContent.GetChild(i).gameObject);
        }

        var incoming = await SupabaseManager.Instance.FriendRequest.GetIncomingRequests();
        var pending = incoming
            .Where(x => x.Accepted == null)
            .OrderByDescending(x => x.CreatedAt)
            .GroupBy(x => x.SenderId)
            .Select(x => x.First())
            .ToList();

        foreach (var request in pending) {
            var user = await SupabaseManager.Instance.User.LoadUserById(request.SenderId);
            if (user == null) continue;

            if (!_friendRequestStateByUserId.TryGetValue(user.id, out var state)) {
                state = new UserFriendRequestState();
                _friendRequestStateByUserId[user.id] = state;
            }
            state.hasIncomingRequest = true;
            state.isFriend = false;

            var cardObject = Instantiate(_receivedRequestUserCardPrefab, _receivedRequestsContent);
            if (!cardObject.TryGetComponent<IUserCard>(out var card)) continue;
            card.Setup(user, this);
        }
    }

    public UserFriendRequestState GetFriendRequestState(System.Guid userId) {
        if (_friendRequestStateByUserId.TryGetValue(userId, out var state)) {
            return state;
        }

        return new UserFriendRequestState {
            isFriend = false,
            hasIncomingRequest = false,
            hasOutgoingRequest = false,
        };
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
            await RefreshFriendRequests();
            await RefreshFriends();
            return false;
        }

        await SupabaseManager.Instance.FriendRequest.SendRequest(userId);
        await RefreshFriendRequests();
        await RefreshFriends();
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
        await RefreshFriendRequests();
        await RefreshFriends();
        return false;
    }

    public async Task OnRemoveFriend(System.Guid userId) {
        await SupabaseManager.Instance.Friendship.DeleteFriendship(userId);
        await RefreshFriendRequests();
        await RefreshFriends();
    }

    public async Task<bool> OnToggleBlockUser(System.Guid userId, bool isBlocked) {
        if (isBlocked) {
            await SupabaseManager.Instance.Block.UnblockUser(userId);
            await RefreshBlockedUsers();
            return false;
        }

        await SupabaseManager.Instance.Block.BlockUser(userId);
        await RefreshBlockedUsers();
        return true;
    }

    [Serializable]
    class AuthError {
        public string msg;
    }
}
