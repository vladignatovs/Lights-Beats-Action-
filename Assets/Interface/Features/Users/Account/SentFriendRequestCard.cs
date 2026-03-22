using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SentFriendRequestCard : MonoBehaviour, IUserCard {
    [SerializeField] TMP_Text _usernameText;
    [SerializeField] Button _friendRequestButton;
    [SerializeField] TMP_Text _friendRequestButtonText;

    UserMetadata _metadata;
    IUserCardCallbacks _callbacks;

    public void Setup(UserMetadata metadata, IUserCardCallbacks callbacks) {
        _metadata = metadata;
        _callbacks = callbacks;

        _usernameText.text = _metadata.username;
        RefreshButtonText();

        _friendRequestButton.onClick.RemoveAllListeners();
        _friendRequestButton.onClick.AddListener(async () => await ToggleRequest());
    }

    async Task ToggleRequest() {
        var state = _callbacks.GetFriendRequestState(_metadata.id);
        state.hasOutgoingRequest = await _callbacks.OnToggleFriendRequest(_metadata.id, state.hasOutgoingRequest);
        RefreshButtonText();
    }

    void RefreshButtonText() {
        var state = _callbacks.GetFriendRequestState(_metadata.id);
        _friendRequestButtonText.text = state.hasOutgoingRequest ? "Cancel" : "Send Request";
    }
}
