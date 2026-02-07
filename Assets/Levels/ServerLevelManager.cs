using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class ServerLevelManager {
    static LevelCreator _levelCreator = new();

    public async Task<(List<LevelMetadata> items, int totalCount)> LazyLoadLevels(int offset, int limit) {
        return await SupabaseManager.Instance.Level.LazyLoadLevels(offset, limit);
    }

    public async Task<List<LevelMetadata>> LazyLoadOwnedLevels() {
        return await SupabaseManager.Instance.Level.LazyLoadOwnedLevels();
    }

    public async Task<Level> LoadLevel(Guid serverId) {
        return await SupabaseManager.Instance.Level.LoadLevel(serverId);
    }

    public static async Task<Level> PublishLevel(Level level) {
        try {
            return await SupabaseManager.Instance.Level.PublishLevel(level);
        } catch (Exception e) {
            Debug.LogWarning("[ServerLevelManager] " + e);
            return null;
        }
    }

    public async Task ImportLevel(Guid? serverId) {
        var level = await SupabaseManager.Instance.Level.LoadLevel(serverId.Value);
        level.id = _levelCreator.GetNextId();
        await _levelCreator.WriteLevelFile(level);
    }

    public async Task DeleteLevel(Guid? serverId) {
        await SupabaseManager.Instance.Level.DeleteLevel(serverId.Value);
    }
}