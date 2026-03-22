using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BlockedUserCard : MonoBehaviour, IUserCard {
    [SerializeField] TMP_Text _usernameText;
    [SerializeField] Button _blockButton;
    [SerializeField] TMP_Text _blockButtonText;

    UserMetadata _metadata;
    IUserCardCallbacks _callbacks;

    public void Setup(UserMetadata metadata, IUserCardCallbacks callbacks) {
        _metadata = metadata;
        _callbacks = callbacks;

        _usernameText.text = _metadata.username;
        RefreshBlockButtonText();

        _blockButton.onClick.RemoveAllListeners();
        _blockButton.onClick.AddListener(async () => await ToggleBlockedState());
    }

    async Task ToggleBlockedState() {
        _metadata.isBlocked = await _callbacks.OnToggleBlockUser(_metadata.id, _metadata.isBlocked);
        RefreshBlockButtonText();
    }

    void RefreshBlockButtonText() {
        _blockButtonText.text = _metadata.isBlocked ? "Unblock" : "Block";
    }
}
