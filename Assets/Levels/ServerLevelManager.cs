using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ServerLevelManager : BaseLevelManager<Guid> {
    public override async Task<List<LevelMetadata>> LazyLoadLevels() {
        Debug.Log("[ServerLevelManager]: fetching levels");
        return await SupabaseManager.Instance.Level.LazyLoadLevels();
    }

    public async Task<List<LevelMetadata>> LazyLoadOwnedLevels() {
        Debug.Log("[ServerLevelManager]: fetching owned levels");
        return await SupabaseManager.Instance.Level.LazyLoadOwnedLevels();
    }

    public override async Task<Level> LoadLevel(Guid serverId) {
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
}