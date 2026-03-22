using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Supabase;

public class FriendshipManager : DataManager {
    public event System.Action OnFriendshipsChanged;

    public FriendshipManager(Client client) : base(client) {
    }

    public async Task<List<FriendshipRecord>> GetMyFriendships() {
        var currentUserId = _client.Auth.CurrentUser?.Id;
        if (!Guid.TryParse(currentUserId, out var currentUserGuid)) {
            return new List<FriendshipRecord>();
        }

        var outgoingTask = _client
            .From<FriendshipRecord>()
            .Where(x => x.FriendId == currentUserGuid)
            .Get();

        var incomingTask = _client
            .From<FriendshipRecord>()
            .Where(x => x.FriendedId == currentUserGuid)
            .Get();

        await Task.WhenAll(outgoingTask, incomingTask);

        var outgoing = (await outgoingTask).Models ?? new List<FriendshipRecord>();
        var incoming = (await incomingTask).Models ?? new List<FriendshipRecord>();
        return outgoing.Concat(incoming).ToList();
    }

    public async Task DeleteFriendship(Guid otherUserId) {
        var currentUserId = _client.Auth.CurrentUser?.Id;
        if (!Guid.TryParse(currentUserId, out var me)) {
            return;
        }

        await _client
            .From<FriendshipRecord>()
            .Where(x => x.FriendId == me && x.FriendedId == otherUserId)
            .Delete();

        await _client
            .From<FriendshipRecord>()
            .Where(x => x.FriendId == otherUserId && x.FriendedId == me)
            .Delete();

        OnFriendshipsChanged?.Invoke();
    }
}
