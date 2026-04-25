using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessengerManager : MonoBehaviour {
    public static MessengerManager Instance { get; private set; }

    [SerializeField] ChatMessage _chatMessagePrefab;
    [SerializeField] Transform _chatContent;
    [SerializeField] TMP_Text _emptyMessengerText;
    [SerializeField] TMP_Text _emptyChatText;
    [SerializeField] TMP_InputField _inputField;
    [SerializeField] ScrollRect _scrollRect;
    [SerializeField] TMP_InputField _editInputField;
    [SerializeField] Button _cancelEditButton;
    [SerializeField] int _maxMessageLength = 280;

    readonly HashSet<long> _renderedMessageIds = new();
    readonly ConcurrentQueue<PendingMessageChange> _pendingMessages = new();
    readonly Dictionary<long, ChatMessage> _renderedMessages = new();

    Guid? _activeChatUserId;
    string _activeChatUsername;

    void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start() {
        _inputField.onSubmit.AddListener(HandleInputSubmit);
        _cancelEditButton.onClick.AddListener(CancelEdit);
        ToggleEditMode(false);
        ToggleEmptyMessengerState(true);
        ToggleEmptyChatState(false);

        SupabaseManager.Instance.Auth.OnAuthenticated += HandleAuthenticated;

        if (SupabaseManager.Instance.Auth.IsAuthenticated) {
            HandleAuthenticated();
        }
    }

    void OnDestroy() {
        if (SupabaseManager.Instance != null && SupabaseManager.Instance.Message != null) {
            SupabaseManager.Instance.Message.OnMessageInserted -= HandleMessageInserted;
            SupabaseManager.Instance.Message.OnMessageUpdated -= HandleMessageUpdated;
            SupabaseManager.Instance.Message.OnMessageDeleted -= HandleMessageDeleted;
        }

        _inputField.onSubmit.RemoveListener(HandleInputSubmit);
        _editInputField.onSubmit.RemoveAllListeners();
        _cancelEditButton.onClick.RemoveListener(CancelEdit);

        if (SupabaseManager.Instance != null && SupabaseManager.Instance.Auth != null) {
            SupabaseManager.Instance.Auth.OnAuthenticated -= HandleAuthenticated;
        }

        if (Instance == this) {
            Instance = null;
        }
    }

    void HandleInputSubmit(string _) {
        SendCurrentMessage();
    }

    void Update() {
        while (_pendingMessages.TryDequeue(out var change)) {
            if (!_activeChatUserId.HasValue) {
                continue;
            }

            if (change.Type != MessageChangeType.Deleted && !IsForActiveChat(change.Message)) {
                continue;
            }

            ApplyMessageChange(change);
        }
    }

    void HandleAuthenticated() {
        SupabaseManager.Instance.Message.OnMessageInserted -= HandleMessageInserted;
        SupabaseManager.Instance.Message.OnMessageUpdated -= HandleMessageUpdated;
        SupabaseManager.Instance.Message.OnMessageDeleted -= HandleMessageDeleted;
        SupabaseManager.Instance.Message.OnMessageInserted += HandleMessageInserted;
        SupabaseManager.Instance.Message.OnMessageUpdated += HandleMessageUpdated;
        SupabaseManager.Instance.Message.OnMessageDeleted += HandleMessageDeleted;
    }

    public async void OpenChat(Guid receiverId, string receiverUsername) {
        _activeChatUserId = receiverId;
        _activeChatUsername = receiverUsername;
        ToggleEmptyMessengerState(false);
        await LoadChat(receiverId);
        await SupabaseManager.Instance.Message.SubscribeToConversation(receiverId);
    }

    public void CloseChat() {
        _activeChatUserId = null;
        _activeChatUsername = null;
        _renderedMessageIds.Clear();
        _renderedMessages.Clear();
        ClearRenderedMessages();
        ToggleEmptyMessengerState(true);
        ToggleEmptyChatState(false);
        SupabaseManager.Instance.Message.Unsubscribe();
    }

    public bool HasActiveChat => _activeChatUserId.HasValue;

    public async void SendCurrentMessage() {
        if (!_activeChatUserId.HasValue) {
            return;
        }

        string text = _inputField.text.Trim();
        if (string.IsNullOrWhiteSpace(text)) {
            return;
        }

        if (text.Length > _maxMessageLength) {
            text = text[.._maxMessageLength];
        }

        _inputField.text = string.Empty;
        await SupabaseManager.Instance.Message.SendMessage(_activeChatUserId.Value, text);
    }

    async Task LoadChat(Guid receiverId) {
        ClearRenderedMessages();
        _renderedMessageIds.Clear();
        _renderedMessages.Clear();

        List<Message> messages = await SupabaseManager.Instance.Message.LoadConversation(receiverId);
        ToggleEmptyChatState(messages.Count == 0);
        foreach (var message in messages) {
            RenderMessage(message);
        }

        ScrollToBottom();
    }

    void HandleMessageInserted(Message message) {
        _pendingMessages.Enqueue(new PendingMessageChange(MessageChangeType.Inserted, message));
    }

    void HandleMessageUpdated(Message message) {
        _pendingMessages.Enqueue(new PendingMessageChange(MessageChangeType.Updated, message));
    }

    void HandleMessageDeleted(Message message) {
        _pendingMessages.Enqueue(new PendingMessageChange(MessageChangeType.Deleted, message));
    }

    bool IsForActiveChat(Message message) {
        if (!Guid.TryParse(SupabaseManager.Instance.Client.Auth.CurrentUser?.Id, out var currentUserId)) {
            return false;
        }

        return
            (message.SenderId == currentUserId && message.ReceiverId == _activeChatUserId.Value) ||
            (message.SenderId == _activeChatUserId.Value && message.ReceiverId == currentUserId);
    }

    void RenderMessage(Message message) {
        if (!_renderedMessageIds.Add(message.Id)) {
            return;
        }

        ToggleEmptyChatState(false);

        var chatMessage = Instantiate(_chatMessagePrefab, _chatContent);
        bool canEdit = message.SenderId.ToString() == SupabaseManager.Instance.Client.Auth.CurrentUser?.Id;
        string senderName = message.SenderId.ToString() == SupabaseManager.Instance.Client.Auth.CurrentUser?.Id
            ? SupabaseManager.Instance.User.Name
            : _activeChatUsername;
        chatMessage.Setup(
            message,
            senderName,
            canEdit,
            () => BeginEditMessage(message),
            () => DeleteMessage(message.Id));
        _renderedMessages[message.Id] = chatMessage;
        ScrollToBottom();
    }

    void ApplyMessageChange(PendingMessageChange change) {
        switch (change.Type) {
            case MessageChangeType.Inserted:
                RenderMessage(change.Message);
                break;
            case MessageChangeType.Updated:
                UpdateRenderedMessage(change.Message);
                break;
            case MessageChangeType.Deleted:
                DeleteRenderedMessage(change.Message.Id);
                break;
        }
    }

    void UpdateRenderedMessage(Message message) {
        if (!_renderedMessages.TryGetValue(message.Id, out var chatMessage)) {
            RenderMessage(message);
            return;
        }

        bool canEdit = message.SenderId.ToString() == SupabaseManager.Instance.Client.Auth.CurrentUser?.Id;
        string senderName = canEdit ? SupabaseManager.Instance.User.Name : _activeChatUsername;
        chatMessage.Setup(
            message,
            senderName,
            canEdit,
            () => BeginEditMessage(message),
            () => DeleteMessage(message.Id));
    }

    void DeleteRenderedMessage(long messageId) {
        _renderedMessageIds.Remove(messageId);

        if (!_renderedMessages.TryGetValue(messageId, out var chatMessage)) {
            return;
        }

        _renderedMessages.Remove(messageId);
        Destroy(chatMessage.gameObject);

        if (_activeChatUserId.HasValue && _renderedMessages.Count == 0) {
            ToggleEmptyChatState(true);
        }
    }

    void BeginEditMessage(Message message) {
        ToggleEditMode(true);
        _editInputField.text = message.Content;
        _editInputField.ActivateInputField();
        _editInputField.onSubmit.RemoveAllListeners();
        _editInputField.onSubmit.AddListener(async _ => {
            string content = _editInputField.text.Trim();
            if (string.IsNullOrWhiteSpace(content)) {
                return;
            }

            await SupabaseManager.Instance.Message.UpdateMessage(message.Id, content);
            ToggleEditMode(false);
        });
    }

    void CancelEdit() {
        ToggleEditMode(false);
    }

    void ToggleEditMode(bool isEditing) {
        if (!isEditing) {
            _editInputField.text = string.Empty;
            _editInputField.onSubmit.RemoveAllListeners();
        }

        _editInputField.gameObject.SetActive(isEditing);
        _cancelEditButton.gameObject.SetActive(isEditing);
        _inputField.gameObject.SetActive(!isEditing);
    }

    async void DeleteMessage(long messageId) {
        await SupabaseManager.Instance.Message.DeleteMessage(messageId);
    }

    void ClearRenderedMessages() {
        for (int i = _chatContent.childCount - 1; i >= 0; i--) {
            Destroy(_chatContent.GetChild(i).gameObject);
        }
    }

    public void ToggleEmptyMessengerState(bool isVisible) {
        _emptyMessengerText.gameObject.SetActive(isVisible);
    }

    public void ToggleEmptyChatState(bool isVisible) {
        _emptyChatText.gameObject.SetActive(isVisible);
    }

    void ScrollToBottom() {
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)_chatContent);
        Canvas.ForceUpdateCanvases();
        StartCoroutine(ScrollToBottomNextFrame());
    }

    IEnumerator ScrollToBottomNextFrame() {
        yield return null;
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)_chatContent);
        Canvas.ForceUpdateCanvases();
        _scrollRect.verticalNormalizedPosition = 0f;
    }

    enum MessageChangeType {
        Inserted,
        Updated,
        Deleted,
    }

    readonly struct PendingMessageChange {
        public MessageChangeType Type { get; }
        public Message Message { get; }

        public PendingMessageChange(MessageChangeType type, Message message) {
            Type = type;
            Message = message;
        }
    }
}
