using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class UserAccountManager : AuthGated, IUserCardCallbacks {
    public override bool ShowOnAuth => true;
    [SerializeField] GameObject _accountPanel;
    [SerializeField] TMP_Text _usernameText;
    [SerializeField] Transform _blockedUsersContent;
    [SerializeField] GameObject _blockedUserCardPrefab;

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
    public async void ToggleAccountPanel() {
        bool shouldOpen = !_accountPanel.activeSelf;
        Overlay.ToggleOverlay(shouldOpen);
        _accountPanel.SetActive(shouldOpen);

        if (shouldOpen) {
            await RefreshBlockedUsers();
        }
    }

    public void SignOut() {
        SupabaseManager.Instance.Auth.SignOut();
        ToggleAccountPanel();
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
}
