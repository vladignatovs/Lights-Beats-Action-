using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class ChatLoader : MonoBehaviour {
    [SerializeField] Transform _contentTransform;
    [SerializeField] GameObject _chatTogglePrefab;

    void Start() {
        SupabaseManager.Instance.Auth.OnAuthenticated += HandleAuthenticated;

        if (SupabaseManager.Instance.Auth.IsAuthenticated) {
            HandleAuthenticated();
        }
    }

    void OnDestroy() {
        if (SupabaseManager.Instance != null && SupabaseManager.Instance.Auth != null) {
            SupabaseManager.Instance.Auth.OnAuthenticated -= HandleAuthenticated;
        }

        if (SupabaseManager.Instance != null && SupabaseManager.Instance.Friendship != null) {
            SupabaseManager.Instance.Friendship.OnFriendshipsChanged -= HandleFriendshipsChanged;
        }
    }

    void HandleAuthenticated() {
        SupabaseManager.Instance.Friendship.OnFriendshipsChanged -= HandleFriendshipsChanged;
        SupabaseManager.Instance.Friendship.OnFriendshipsChanged += HandleFriendshipsChanged;
        RefreshChats();
    }

    async void HandleFriendshipsChanged() {
        await ReloadChats();
    }

    public async void RefreshChats() {
        await ReloadChats();
    }

    async Task ReloadChats() {
        ClearToggles();

        if (!System.Guid.TryParse(SupabaseManager.Instance.Client.Auth.CurrentUser?.Id, out var me)) {
            return;
        }

        var friendships = await SupabaseManager.Instance.Friendship.GetMyFriendships();
        if (friendships.Count == 0) {
            return;
        }

        var friendIds = friendships
            .Select(x => x.FriendId == me ? x.FriendedId : x.FriendId)
            .Distinct()
            .ToList();

        var loadTasks = new List<Task<UserMetadata>>();
        foreach (var friendId in friendIds) {
            loadTasks.Add(SupabaseManager.Instance.User.LoadUserById(friendId));
        }

        var users = await Task.WhenAll(loadTasks);
        RenderChatToggles(users.Where(x => x != null).OrderBy(x => x.username).ToList());
    }

    void RenderChatToggles(List<UserMetadata> users) {
        foreach (var user in users) {
            var toggleObject = Instantiate(_chatTogglePrefab, _contentTransform);
            var toggle = toggleObject.GetComponent<ChatToggle>();
            toggle.Setup(user);
        }
    }

    void ClearToggles() {
        for (int i = _contentTransform.childCount - 1; i >= 0; i--) {
            Destroy(_contentTransform.GetChild(i).gameObject);
        }
    }
}
