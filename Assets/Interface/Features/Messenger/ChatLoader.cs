using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class ChatLoader : MonoBehaviour {
    [SerializeField] Transform _contentTransform;
    [SerializeField] GameObject _chatTogglePrefab;
    [SerializeField] TMP_Text _emptyStateText;

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
    }

    void HandleAuthenticated() {
        RefreshChats();
    }

    public async void RefreshChats() {
        await ReloadChats();
    }

    async Task ReloadChats() {
        ClearToggles();
        ToggleEmptyState(false);

        var participantIds = await SupabaseManager.Instance.Message.LoadConversationUserIds();
        if (participantIds.Count == 0) {
            return;
        }

        var loadTasks = new List<Task<UserMetadata>>();
        foreach (var participantId in participantIds) {
            loadTasks.Add(SupabaseManager.Instance.User.LoadUserById(participantId));
        }

        var users = await Task.WhenAll(loadTasks);
        RenderChatToggles(users.Where(x => x != null).OrderBy(x => x.username).ToList());
        ToggleEmptyState(_contentTransform.childCount == 0);

        if (_contentTransform.childCount == 0) {
            MessengerManager.Instance.ToggleEmptyMessengerState(false);
            return;
        }

        if (!MessengerManager.Instance.HasActiveChat) {
            MessengerManager.Instance.ToggleEmptyMessengerState(true);
        }
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

    void ToggleEmptyState(bool isVisible) {
        _emptyStateText.gameObject.SetActive(isVisible);
    }
}
