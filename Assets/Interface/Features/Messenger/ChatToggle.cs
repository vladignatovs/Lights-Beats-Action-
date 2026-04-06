using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatToggle : MonoBehaviour {
    [SerializeField] Button _button;
    [SerializeField] TMP_Text _usernameText;

    UserMetadata _metadata;

    public void Setup(UserMetadata metadata) {
        _metadata = metadata;

        _usernameText.text = metadata.username;

        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(OpenChat);
    }

    void OpenChat() {
        MessengerManager.Instance.OpenChat(_metadata.id, _metadata.username);
    }
}
