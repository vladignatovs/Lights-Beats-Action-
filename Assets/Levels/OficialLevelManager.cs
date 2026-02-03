using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class OfficialLevelManager : BaseLevelManager<int> {
    const string ResourcesPath = "OfficialLevels";
    LevelCreator _levelCreator = new();

    static List<LevelMetadata> _levelMetadata;
    static Dictionary<int, TextAsset> _idToAsset;

    void EnsureCache() {
        if (_levelMetadata != null) return;

        var assets = Resources.LoadAll<TextAsset>(ResourcesPath);

        _levelMetadata = new();
        _idToAsset = new();

        foreach (var asset in assets) {
            try {
                var levelMetadata = _levelCreator.ReadLevelFileMetadataRaw(asset.text);
                if (levelMetadata != null) {
                    _levelMetadata.Add(levelMetadata);
                    _idToAsset[levelMetadata.id] = asset;
                }
            }
            catch (Exception e) {
                Debug.LogError($"Failed to read official level {asset.name}: {e}");
            }
        }
    }

    public override Task<List<LevelMetadata>> LazyLoadLevels() {
        EnsureCache();
        return Task.FromResult(_levelMetadata);
    }

    public override Task<Level> LoadLevel(int id) {
        EnsureCache();

        if (!_idToAsset.TryGetValue(id, out var asset)) {
            Debug.LogWarning($"Official level with id {id} not found");
            return Task.FromResult<Level>(null);
        }

        return Task.FromResult(_levelCreator.ReadLevelFileRaw(asset.text));
    }
}