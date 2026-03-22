using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Supabase;

public class FriendRequestManager : DataManager {
    public event System.Action OnFriendRequestsChanged;

    public FriendRequestManager(Client client) : base(client) {
    }

    public async Task<FriendRequestRecord> SendRequest(Guid receiverId) {
        var request = new FriendRequestInsert {
            ReceiverId = receiverId,
        };

        var response = await _client
            .From<FriendRequestInsert>()
            .Insert(request);

        OnFriendRequestsChanged?.Invoke();

        var outgoingRequests = await GetOutgoingRequests();
        return outgoingRequests
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefault(x => x.ReceiverId == receiverId);
    }

    public async Task<List<FriendRequestRecord>> GetIncomingRequests() {
        var currentUserId = _client.Auth.CurrentUser?.Id;
        if (!Guid.TryParse(currentUserId, out var currentUserGuid)) {
            return new List<FriendRequestRecord>();
        }

        var response = await _client
            .From<FriendRequestRecord>()
            .Get();

        var requests = response.Models ?? new List<FriendRequestRecord>();
        return requests
            .Where(x => x.ReceiverId == currentUserGuid)
            .ToList();
    }

    public async Task<List<FriendRequestRecord>> GetOutgoingRequests() {
        var currentUserId = _client.Auth.CurrentUser?.Id;
        if (!Guid.TryParse(currentUserId, out var currentUserGuid)) {
            return new List<FriendRequestRecord>();
        }

        var response = await _client
            .From<FriendRequestRecord>()
            .Get();

        var requests = response.Models ?? new List<FriendRequestRecord>();
        return requests
            .Where(x => x.SenderId == currentUserGuid)
            .ToList();
    }

    public async Task<FriendRequestRecord> SetRequestAccepted(long requestId, bool accepted) {
        await _client
            .From<FriendRequestRecord>()
            .Where(x => x.Id == requestId)
            .Set(x => x.Accepted, accepted)
            .Update();

        OnFriendRequestsChanged?.Invoke();

        var incomingRequests = await GetIncomingRequests();
        return incomingRequests.FirstOrDefault(x => x.Id == requestId);
    }

    public async Task DeleteRequest(long requestId) {
        await _client
            .From<FriendRequestRecord>()
            .Where(x => x.Id == requestId)
            .Delete();

        OnFriendRequestsChanged?.Invoke();
    }
}
