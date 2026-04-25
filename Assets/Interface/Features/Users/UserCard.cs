using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserCard : MonoBehaviour, IUserCard {
    [SerializeField] TMP_Text _usernameText;
    [SerializeField] Button _messageButton;
    [SerializeField] Button _blockButton;
    [SerializeField] TMP_Text _blockButtonText;
    [SerializeField] Button _friendRequestButton;
    [SerializeField] TMP_Text _friendRequestButtonText;
    [SerializeField] Button _acceptFriendRequestButton;
    [SerializeField] Button _denyFriendRequestButton;
    [SerializeField] Button _removeFriendButton;

    UserMetadata _metadata;
    IUserCardCallbacks _callbacks;

    public void Setup(UserMetadata metadata, IUserCardCallbacks callbacks) {
        _metadata = metadata;
        _callbacks = callbacks;

        _usernameText.text = metadata.username;
        RefreshBlockButtonText();
        RefreshFriendRequestState();

        _messageButton.onClick.RemoveAllListeners();
        _messageButton.onClick.AddListener(MessageUser);

        _blockButton.onClick.RemoveAllListeners();
        _blockButton.onClick.AddListener(async () => await ToggleBlockedState());

        _friendRequestButton.onClick.RemoveAllListeners();
        _friendRequestButton.onClick.AddListener(async () => await ToggleFriendRequest());

        _acceptFriendRequestButton.onClick.RemoveAllListeners();
        _acceptFriendRequestButton.onClick.AddListener(async () => await RespondToIncomingRequest(true));

        _denyFriendRequestButton.onClick.RemoveAllListeners();
        _denyFriendRequestButton.onClick.AddListener(async () => await RespondToIncomingRequest(false));

        _removeFriendButton.onClick.RemoveAllListeners();
        _removeFriendButton.onClick.AddListener(async () => await RemoveFriend());
    }

    async Task ToggleBlockedState() {
        _metadata.isBlocked = await _callbacks.OnToggleBlockUser(_metadata.id, _metadata.isBlocked);
        RefreshBlockButtonText();
    }

    void RefreshBlockButtonText() {
        if (IsSelf()) {
            _blockButton.gameObject.SetActive(false);
            return;
        }

        _blockButton.gameObject.SetActive(true);
        _blockButtonText.text = _metadata.isBlocked ? "Unblock" : "Block";
    }

    void MessageUser() {
        _callbacks.OnMessageUser(_metadata);
    }

    async Task ToggleFriendRequest() {
        var state = _callbacks.GetFriendRequestState(_metadata.id);
        bool nextOutgoing = await _callbacks.OnToggleFriendRequest(_metadata.id, state.hasOutgoingRequest);
        state.hasOutgoingRequest = nextOutgoing;
        RefreshFriendRequestState();
    }

    async Task RespondToIncomingRequest(bool accept) {
        bool keepIncoming = await _callbacks.OnRespondToFriendRequest(_metadata.id, accept);
        var state = _callbacks.GetFriendRequestState(_metadata.id);
        state.hasIncomingRequest = keepIncoming;
        if (accept) {
            state.isFriend = true;
        }
        RefreshFriendRequestState();
    }

    async Task RemoveFriend() {
        await _callbacks.OnRemoveFriend(_metadata.id);
        var state = _callbacks.GetFriendRequestState(_metadata.id);
        state.isFriend = false;
        RefreshFriendRequestState();
    }

    void RefreshFriendRequestState() {
        if (IsSelf()) {
            _messageButton.gameObject.SetActive(false);
            _friendRequestButton.gameObject.SetActive(false);
            _acceptFriendRequestButton.gameObject.SetActive(false);
            _denyFriendRequestButton.gameObject.SetActive(false);
            _removeFriendButton.gameObject.SetActive(false);
            return;
        }

        var state = _callbacks.GetFriendRequestState(_metadata.id);
        _messageButton.gameObject.SetActive(state.isFriend || SupabaseManager.Instance.User.IsAdmin);

        bool showMainFriendButton = !state.isFriend && !state.hasIncomingRequest;
        bool hideForBlocked = (_metadata.isBlocked || _metadata.hasBlocked) && !state.hasOutgoingRequest;
        _friendRequestButton.gameObject.SetActive(showMainFriendButton && !hideForBlocked);
        _friendRequestButton.interactable = (!_metadata.isBlocked && !_metadata.hasBlocked) || state.hasOutgoingRequest;

        _friendRequestButtonText.text = state.hasOutgoingRequest ? "Cancel" : "Send Request";

        bool showIncomingActions = !state.isFriend && state.hasIncomingRequest;
        _acceptFriendRequestButton.gameObject.SetActive(showIncomingActions);
        _denyFriendRequestButton.gameObject.SetActive(showIncomingActions);

        _removeFriendButton.gameObject.SetActive(state.isFriend);
    }

    bool IsSelf() {
        var currentUserId = SupabaseManager.Instance?.Client?.Auth?.CurrentUser?.Id;
        return System.Guid.TryParse(currentUserId, out var me) && me == _metadata.id;
    }
}
