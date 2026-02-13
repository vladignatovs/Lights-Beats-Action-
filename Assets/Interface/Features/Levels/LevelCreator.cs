using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using SFB;
using UnityEngine;

/// <summary>
/// Class reliable for all .lvl files and reads. All LevelFile and LevelMetadataFile
/// </summary>
public class LevelCreator {
    public ExtensionFilter[] extensions = new[] {
        new ExtensionFilter("Level files", "lvl"),
        new ExtensionFilter("Other", "*")
    };

    internal static readonly JsonSerializerOptions Options = new() {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new FloatRoundingConverter() }
    };

    // TODO: handle guest level migration to the first authenticated account
    string LevelsFolder => Path.Combine(
        Application.persistentDataPath, 
        SupabaseManager.Instance.Client.Auth.CurrentUser?.Id ?? "Guest", 
        "Levels");

    string GetLevelPath(int id) => Path.Combine(LevelsFolder, $"{id}.lvl");
    int _lastId = 0;
    // TODO: fix the bug with new levels carrying the last id from the previous account if changed
    public int GetNextId() {
        if (!Directory.Exists(LevelsFolder))
            return ++_lastId;

        int maxId = Directory.GetFiles(LevelsFolder, "*.lvl")
            .Select(file => {
                string json = File.ReadAllText(file);
                var meta = JsonSerializer.Deserialize<LevelFileMetadata>(json, Options)?.Meta;
                return meta?.id ?? 0;
            })
            .DefaultIfEmpty(0)
            .Max();

        _lastId = Math.Max(_lastId, maxId) + 1;
        return _lastId;
    }

    public Level ReadLevelFileRaw(string text) {
        var levelFile = JsonSerializer.Deserialize<LevelFile>(text, Options);
        return levelFile.ToLevel();
    }

    public async Task<Level> ReadLevelFile(string path) {
        using var stream = File.OpenRead(path);
        var levelFile = await JsonSerializer.DeserializeAsync<LevelFile>(stream, Options);
        return levelFile.ToLevel();
    }

    public async Task<Level> ReadLevelFile(int id) {
        return await ReadLevelFile(GetLevelPath(id));
    }

    public LevelMetadata ReadLevelFileMetadataRaw(string text) {
        var levelFileMetadata = JsonSerializer.Deserialize<LevelFileMetadata>(text, Options);
        return levelFileMetadata?.Meta;
    }

    public async Task<LevelMetadata> ReadLevelFileMetadata(string path) {
        using var stream = File.OpenRead(path);
        var levelFileMetadata = await JsonSerializer.DeserializeAsync<LevelFileMetadata>(stream, Options);
        return levelFileMetadata?.Meta;
    }

    /// <summary>
    /// Read only the level metadata of all local levels
    /// </summary>
    /// <returns></returns>
    public async Task<List<LevelMetadata>> ReadLevelFileMetadata() {
        if (!Directory.Exists(LevelsFolder)) {
            Debug.LogWarning("Levels folder not found");
            return new();
        }
        string[] files = Directory.GetFiles(LevelsFolder, "*.lvl", SearchOption.TopDirectoryOnly);
        var tasks = files.Select(file => ReadLevelFileMetadata(file));
        var results = await Task.WhenAll(tasks);
        
        return results.Where(x => x != null).ToList();
    }
    
    /// <summary>
    /// Read only the level metadata files for a specific page (offset + limit)
    /// Uses EnumerateFiles for lazy evaluation - only iterates through file paths once
    /// </summary>
    public async Task<(List<LevelMetadata> items, int totalCount)> ReadLevelFileMetadata(int offset, int limit) {
        if (!Directory.Exists(LevelsFolder)) {
            Debug.LogWarning("Levels folder not found");
            return (new(), 0);
        }
        
        // TODO: enumerate files returns files by alphabetic order, so 19.lvl comes before 2.lvl, hence the unexpected sorting of levels
        // should be fixed to show in creation order
        // Use EnumerateFiles for lazy evaluation
        var filePaths = Directory.EnumerateFiles(LevelsFolder, "*.lvl", SearchOption.TopDirectoryOnly);
        
        // Count total while collecting only the page we need (single pass)
        int totalCount = 0;
        var pagePaths = new List<string>();
        
        foreach (var path in filePaths) {
            if (totalCount >= offset && totalCount < offset + limit) {
                pagePaths.Add(path);
            }
            totalCount++;
        }
        
        // Only read the metadata for files in this page
        var tasks = pagePaths.Select(path => ReadLevelFileMetadata(path));
        var results = await Task.WhenAll(tasks);
        
        return (results.Where(x => x != null).ToList(), totalCount);
    }

    public async Task WriteLevelFile(Level level, string path) {

        var temp = path + ".tmp";

        var json = JsonSerializer.Serialize(level.ToLevelFile(), Options);
        Debug.Log(json);
        // writes to temporary files to prevent crashes mid write
        await File.WriteAllTextAsync(temp, json);

        // replaces if file exists, else just "moves" the temp file to path
        if (File.Exists(path))
            File.Replace(temp, path, null); // windows only (i think)
        else
            File.Move(temp, path);
    }

    /// <summary>
    /// Overload of the WriteLevelFile which should be used when writing a LocalLevel,
    /// </summary>
    /// <param name="levelFile"></param>
    /// <returns></returns>
    public async Task WriteLevelFile(Level level) {
        if (!Directory.Exists(LevelsFolder)) {
            Directory.CreateDirectory(LevelsFolder);
        }
        await WriteLevelFile(level, GetLevelPath(level.id));
    }

    public void DeleteLevelFile(int id) {
        var path = GetLevelPath(id);
        if (File.Exists(path)) {
            File.Delete(path);
        }
    }

    public async Task ExportLevelFile(Level level) {
        var path = StandaloneFileBrowser.SaveFilePanel("Open Files", "", level.name, extensions);
        if (string.IsNullOrWhiteSpace(path)) return;
        // write the level file
        await WriteLevelFile(level, path);
    }

    public async Task ImportLevelFile() {
        var paths = StandaloneFileBrowser.OpenFilePanel("Open Files", "", extensions, false);
        if (paths.Length == 0) return;
        var level = await ReadLevelFile(paths[0]);
        level.id = GetNextId();
        await WriteLevelFile(level);
    }
}

public class FloatRoundingConverter : JsonConverter<float> {
    public override float Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        return reader.GetSingle();
    }

    public override void Write(Utf8JsonWriter writer, float value, JsonSerializerOptions options) {
        decimal rounded = (decimal)Math.Round(value, 3);
        writer.WriteNumberValue(rounded);
    }
}

public class LevelFile {
    public LevelMetadata Meta { get; set; }
    public List<Action> Actions { get; set; }

    public Level ToLevel() {
        return new Level {
            id = Meta.id,
            serverId = Meta.serverId,
            name = Meta.name,
            bpm = Meta.bpm,
            startOffset = Meta.startOffset,
            audioPath = Meta.audioPath,
            actions = Actions
        };
    }
}

public class LevelFileMetadata {
    public LevelMetadata Meta { get; set; }
}

public static class Utils {
    public static LevelFile ToLevelFile(this Level level) {
        return new() {
            Meta = new LevelMetadata {
                id = level.id,
                serverId = level.serverId,
                name = level.name,
                bpm = level.bpm,
                startOffset = level.startOffset,
                audioPath = level.audioPath
            },
            Actions = level.actions
        };
    }
}