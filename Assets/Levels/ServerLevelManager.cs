using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ServerLevelManager : IPageProvider<LevelMetadata> {
    static LevelCreator _levelCreator = new();

    public async Task<(List<LevelMetadata> items, int totalCount)> LoadPage(int offset, int pageSize, List<IFilter> filters = null) {
        // Convert generic IFilter<LevelMetadata> to data layer IDataFilter<ServerLevelMetadata>
        List<IDataFilter<ServerLevelMetadata>> dataFilters = null;
        if (filters != null) {
            dataFilters = new List<IDataFilter<ServerLevelMetadata>>();
            foreach (var filter in filters) {
                if (filter is IDataFilter<ServerLevelMetadata> dataFilter) {
                    dataFilters.Add(dataFilter);
                }
            }
        }
        
        return await SupabaseManager.Instance.Level.LazyLoadLevels(offset, pageSize, dataFilters);
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