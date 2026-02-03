using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Handles the logic of CRUD of the Local Levels, e.g. levels created by the user and stored locally on the machine of the user
/// handles the level creations, saving, deletion
/// </summary>
public class LocalLevelManager : BaseLevelManager<int> {
    LevelCreator _levelCreator = new();
    public override async Task<List<LevelMetadata>> LazyLoadLevels() {
        return await _levelCreator.ReadLevelFileMetadata();
    }

    public override async Task<Level> LoadLevel(int id) {
        try {
            return await _levelCreator.ReadLevelFile(id);
        } catch {
            Debug.LogError($"Failed to load level {id}");
            return null;
        }
    }

    public static async Task CreateNewLevel(string name, float bpm, string audioPath) {
        Level level = new() {
            id = LevelCreator.GetNextId(),
            name = name,
            bpm = bpm,
            audioPath = audioPath,
            actions = new()
        };
        await SaveLevel(level);
    }

    public static async Task SaveLevel(Level level) {
        await LevelCreator.WriteLevelFile(level);
    }

    public static void DeleteLevel(int id) {
        LevelCreator.DeleteLevelFile(id);
    }

    public async Task ExportLevel(int id) {
        var level = await _levelCreator.ReadLevelFile(id);
        await _levelCreator.ExportLevelFile(level);
    }

    public async Task ImportLevel() {
        await _levelCreator.ImportLevelFile();
    }
}