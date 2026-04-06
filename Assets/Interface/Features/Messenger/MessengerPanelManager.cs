using UnityEngine;

public class MessengerPanelManager : MonoBehaviour {
    [SerializeField] GameObject _panel;
    [SerializeField] ChatLoader _chatLoader;
    [SerializeField] MessengerManager _messengerManager;

    public void TogglePanel() {
        bool shouldOpen = !_panel.activeSelf;
        _panel.SetActive(shouldOpen);
        Overlay.ToggleOverlay(shouldOpen);

        if (shouldOpen) {
            _chatLoader.RefreshChats();
        } else {
            _messengerManager.CloseChat();
        }
    }
}
