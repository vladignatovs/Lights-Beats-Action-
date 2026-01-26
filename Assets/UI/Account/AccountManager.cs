using TMPro;
using UnityEngine;

public class AccountManager : AuthGated {
    public override bool ShowOnAuth => true;
    [SerializeField] GameObject _accountPanel;
    [SerializeField] TMP_Text _usernameText;
    [SerializeField] Overlay _overlay;

    protected override void Start() {
        base.Start();
        // set immediately from cache
        _usernameText.SetText(SupabaseManager.Instance.User.Name);
        // subscribe for updates
        SupabaseManager.Instance.User.OnNameChanged += HandleNameChanged;
    }

    protected override void OnDestroy() {
        base.OnDestroy();
        SupabaseManager.Instance.User.OnNameChanged -= HandleNameChanged;
    }

    void HandleNameChanged(string name) {
        _usernameText.SetText(name);
    }
    public void ToggleAccountPanel() {
        _overlay.ToggleOverlay(!_accountPanel.activeSelf);
        _accountPanel.SetActive(!_accountPanel.activeSelf);
    }

    public void SignOut() {
        SupabaseManager.Instance.Auth.SignOut();
        ToggleAccountPanel();
    }
}