using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FriendUserCard : MonoBehaviour, IUserCard {
    [SerializeField] TMP_Text _usernameText;
    [SerializeField] Button _removeFriendButton;

    UserMetadata _metadata;
    IUserCardCallbacks _callbacks;

    public void Setup(UserMetadata metadata, IUserCardCallbacks callbacks) {
        _metadata = metadata;
        _callbacks = callbacks;

        _usernameText.text = _metadata.username;

        _removeFriendButton.onClick.RemoveAllListeners();
        _removeFriendButton.onClick.AddListener(async () => await RemoveFriend());
    }

    async Task RemoveFriend() {
        await _callbacks.OnRemoveFriend(_metadata.id);
    }
}
