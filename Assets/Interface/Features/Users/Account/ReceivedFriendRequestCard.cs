using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReceivedFriendRequestCard : MonoBehaviour, IUserCard {
    [SerializeField] TMP_Text _usernameText;
    [SerializeField] Button _acceptButton;
    [SerializeField] Button _denyButton;

    UserMetadata _metadata;
    IUserCardCallbacks _callbacks;

    public void Setup(UserMetadata metadata, IUserCardCallbacks callbacks) {
        _metadata = metadata;
        _callbacks = callbacks;

        _usernameText.text = _metadata.username;

        _acceptButton.onClick.RemoveAllListeners();
        _acceptButton.onClick.AddListener(async () => await Respond(true));

        _denyButton.onClick.RemoveAllListeners();
        _denyButton.onClick.AddListener(async () => await Respond(false));
    }

    async Task Respond(bool accept) {
        await _callbacks.OnRespondToFriendRequest(_metadata.id, accept);
    }
}
