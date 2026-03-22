using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Supabase;

public class BlockManager : DataManager {
    public event System.Action OnBlocksChanged;

    public BlockManager(Client client) : base(client) {
    }

    public async Task<BlockRecord> BlockUser(Guid blockedUserId) {
        var block = new BlockRecord {
            Blocked = blockedUserId
        };

        var response = await _client
            .From<BlockRecord>()
            .Insert(block);

        OnBlocksChanged?.Invoke();

        return response.Model;
    }

    public async Task UnblockUser(Guid blockedUserId) {
        await _client
            .From<BlockRecord>()
            .Where(x => x.Blocked == blockedUserId)
            .Delete();

        OnBlocksChanged?.Invoke();
    }

    public async Task<HashSet<Guid>> GetBlockedUserIds() {
        var response = await _client
            .From<BlockRecord>()
            .Select("blocked_id")
            .Get();

        return response.Models
            .Select(x => x.Blocked)
            .ToHashSet();
    }

    public async Task<HashSet<Guid>> GetUsersWhoBlockedMeIds() {
        var currentUserId = _client.Auth.CurrentUser?.Id;
        if (!Guid.TryParse(currentUserId, out var me)) {
            return new HashSet<Guid>();
        }

        var response = await _client
            .From<BlockRelationRecord>()
            .Where(x => x.Blocked == me)
            .Select("blocker_id,blocked_id")
            .Get();

        return response.Models
            .Select(x => x.Blocker)
            .ToHashSet();
    }
}
