using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Supabase.Realtime;
using Supabase.Realtime.Interfaces;
using Supabase.Realtime.PostgresChanges;

public class MessageManager : DataManager {
    IRealtimeChannel _channel;
    Guid? _activeUserA;
    Guid? _activeUserB;

    public event Action<Message> OnMessageInserted;
    public event Action<Message> OnMessageUpdated;
    public event Action<Message> OnMessageDeleted;

    public MessageManager(Supabase.Client client) : base(client) {
    }

    public async Task<List<Message>> LoadConversation(Guid otherUserId) {
        if (!Guid.TryParse(_client.Auth.CurrentUser?.Id, out var currentUserId)) {
            return new List<Message>();
        }

        var sentTask = _client
            .From<Message>()
            .Where(x => x.SenderId == currentUserId)
            .Where(x => x.ReceiverId == otherUserId)
            .Get();

        var receivedTask = _client
            .From<Message>()
            .Where(x => x.SenderId == otherUserId)
            .Where(x => x.ReceiverId == currentUserId)
            .Get();

        await Task.WhenAll(sentTask, receivedTask);

        var sent = (await sentTask).Models ?? new List<Message>();
        var received = (await receivedTask).Models ?? new List<Message>();

        return sent
            .Concat(received)
            .OrderBy(x => x.CreatedAt)
            .ThenBy(x => x.Id)
            .ToList();
    }

    public async Task<Message> SendMessage(Guid receiverId, string text) {
        if (!Guid.TryParse(_client.Auth.CurrentUser?.Id, out var senderId)) {
            throw new InvalidOperationException("Cannot send a message without an authenticated user.");
        }

        var trimmedText = text.Trim();
        var payload = new MessageInsert {
            SenderId = senderId,
            ReceiverId = receiverId,
            Content = trimmedText,
            CreatedAt = DateTime.UtcNow,
        };

        var response = await _client.From<Message>().Insert(new Message {
            SenderId = payload.SenderId,
            ReceiverId = payload.ReceiverId,
            Content = payload.Content,
            CreatedAt = payload.CreatedAt,
        });

        return response.Models[0];
    }

    public async Task SubscribeToConversation(Guid otherUserId) {
        if (!Guid.TryParse(_client.Auth.CurrentUser?.Id, out var currentUserId)) {
            return;
        }

        await _client.Realtime.ConnectAsync();

        if (_activeUserA == currentUserId && _activeUserB == otherUserId) {
            return;
        }

        Unsubscribe();

        var ordered = OrderParticipants(currentUserId, otherUserId);
        _activeUserA = ordered.Item1;
        _activeUserB = ordered.Item2;

        _channel = _client.Realtime.Channel($"realtime:message:{_activeUserA}:{_activeUserB}");
        _channel.Register(new PostgresChangesOptions("public", "message"));
        _channel.AddPostgresChangeHandler(PostgresChangesOptions.ListenType.Inserts, HandleMessageInserted);
        _channel.AddPostgresChangeHandler(PostgresChangesOptions.ListenType.Updates, HandleMessageUpdated);
        _channel.AddPostgresChangeHandler(PostgresChangesOptions.ListenType.Deletes, HandleMessageDeleted);

        await _channel.Subscribe();
    }

    public async Task UpdateMessage(long messageId, string content) {
        await _client
            .From<Message>()
            .Where(x => x.Id == messageId)
            .Set(x => x.Content, content.Trim())
            .Update();
    }

    public async Task DeleteMessage(long messageId) {
        await _client
            .From<Message>()
            .Where(x => x.Id == messageId)
            .Delete();
    }

    public void Unsubscribe() {
        if (_channel != null) {
            _channel.Unsubscribe();
            _client.Realtime.Remove((RealtimeChannel)_channel);
            _channel = null;
        }

        _activeUserA = null;
        _activeUserB = null;
    }

    void HandleMessageInserted(IRealtimeChannel sender, PostgresChangesResponse change) {
        var message = change.Model<Message>();
        if (message == null) {
            return;
        }

        if (!IsActiveConversation(message.SenderId, message.ReceiverId)) {
            return;
        }

        OnMessageInserted?.Invoke(message);
    }

    void HandleMessageUpdated(IRealtimeChannel sender, PostgresChangesResponse change) {
        var message = change.Model<Message>();
        if (message == null) {
            return;
        }

        if (!IsActiveConversation(message.SenderId, message.ReceiverId)) {
            return;
        }

        OnMessageUpdated?.Invoke(message);
    }

    void HandleMessageDeleted(IRealtimeChannel sender, PostgresChangesResponse change) {
        var message = change.OldModel<Message>() ?? change.Model<Message>();
        if (message == null) {
            return;
        }

        OnMessageDeleted?.Invoke(message);
    }

    bool IsActiveConversation(Guid senderId, Guid receiverId) {
        if (!_activeUserA.HasValue || !_activeUserB.HasValue) {
            return false;
        }

        var ordered = OrderParticipants(senderId, receiverId);
        return ordered.Item1 == _activeUserA.Value && ordered.Item2 == _activeUserB.Value;
    }

    static (Guid, Guid) OrderParticipants(Guid left, Guid right) {
        return string.CompareOrdinal(left.ToString("N"), right.ToString("N")) <= 0
            ? (left, right)
            : (right, left);
    }
}
