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
/// Interface for level serialization/deserialization. 
/// Implement this to support different file formats (JSON, Binary, XML, etc.)
/// </summary>
public interface ILevelSerializer {
    /// <summary>
    /// Serializes a Level to a string representation
    /// </summary>
    string Serialize(Level level);

    /// <summary>
    /// Serializes a Level to a string representation asynchronously
    /// </summary>
    Task<string> SerializeAsync(Level level);

    /// <summary>
    /// Deserializes a Level from a string representation
    /// </summary>
    Level Deserialize(string data);

    /// <summary>
    /// Deserializes a Level from a string representation asynchronously
    /// </summary>
    Task<Level> DeserializeAsync(string data);

    /// <summary>
    /// Deserializes only the metadata from a string representation
    /// </summary>
    LevelMetadata DeserializeMetadata(string data);

    /// <summary>
    /// Deserializes only the metadata from a string representation asynchronously
    /// </summary>
    Task<LevelMetadata> DeserializeMetadataAsync(string data);

    /// <summary>
    /// Gets the file extension for this serializer (e.g., "lvl", "json", "bin")
    /// </summary>
    string FileExtension { get; }

    /// <summary>
    /// Gets a human-readable name for this format (e.g., "Level files", "JSON files")
    /// </summary>
    string FormatName { get; }
}

/// <summary>
/// JSON implementation of ILevelSerializer using System.Text.Json
/// </summary>
public class JsonLevelSerializer : ILevelSerializer {
    public string FileExtension => "lvl";
    public string FormatName => "Level files";

    internal static readonly JsonSerializerOptions Options = new() {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new FloatRoundingConverter() }
    };

    public string Serialize(Level level) {
        return JsonSerializer.Serialize(level.ToLevelFile(), Options);
    }

    // TODO: clean up level serializer, turn it into a proper extend of JsonSerializer
    public async Task<string> SerializeAsync(Level level) {
        return await Task.Run(() => Serialize(level));
    }

    public Level Deserialize(string data) {
        var levelFile = JsonSerializer.Deserialize<LevelFile>(data, Options);
        return levelFile?.ToLevel();
    }

    public async Task<Level> DeserializeAsync(string data) {
        return await Task.Run(() => Deserialize(data));
    }

    public LevelMetadata DeserializeMetadata(string data) {
        var levelFileMetadata = JsonSerializer.Deserialize<LevelFileMetadata>(data, Options);
        return levelFileMetadata?.Meta;
    }

    public async Task<LevelMetadata> DeserializeMetadataAsync(string data) {
        return await Task.Run(() => DeserializeMetadata(data));
    }
}

/// <summary>
/// Class responsible for all .lvl file operations (read/write/import/export).
/// Acts as a wrapper around ILevelSerializer for file I/O operations.
/// </summary>
public class LevelCreator {
    private readonly ILevelSerializer _serializer;

    public ExtensionFilter[] extensions => new[] {
        new ExtensionFilter(_serializer.FormatName, _serializer.FileExtension),
        new ExtensionFilter("Other", "*")
    };

    /// <summary>
    /// Creates a new LevelCreator with the default JSON serializer
    /// </summary>
    public LevelCreator() : this(new JsonLevelSerializer()) { }

    /// <summary>
    /// Creates a new LevelCreator with a custom serializer
    /// </summary>
    /// <param name="serializer">The serializer to use for level file operations</param>
    public LevelCreator(ILevelSerializer serializer) {
        _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
    }

    // TODO: handle guest level migration to the first authenticated account
    static string LevelsFolder => Path.Combine(
        Application.persistentDataPath, 
        SupabaseManager.Instance.Client.Auth.CurrentUser?.Id ?? "Guest", 
        "Levels");

    string GetLevelPath(int id) => Path.Combine(LevelsFolder, $"{id}.{_serializer.FileExtension}");
    static int _lastId = 0;

    public int GetNextId() {
        if (!Directory.Exists(LevelsFolder))
            return ++_lastId;

        int maxId = Directory.GetFiles(LevelsFolder, $"*.{_serializer.FileExtension}")
            .Select(file => {
                string data = File.ReadAllText(file);
                var meta = _serializer.DeserializeMetadata(data);
                return meta?.id ?? 0;
            })
            .DefaultIfEmpty(0)
            .Max();

        _lastId = Math.Max(_lastId, maxId) + 1;
        return _lastId;
    }

    public Level ReadLevelFileRaw(string text) {
        return _serializer.Deserialize(text);
    }

    public async Task<Level> ReadLevelFile(string path) {
        string data = await File.ReadAllTextAsync(path);
        return await _serializer.DeserializeAsync(data);
    }

    public async Task<Level> ReadLevelFile(int id) {
        return await ReadLevelFile(GetLevelPath(id));
    }

    public LevelMetadata ReadLevelFileMetadataRaw(string text) {
        return _serializer.DeserializeMetadata(text);
    }

    public async Task<LevelMetadata> ReadLevelFileMetadata(string path) {
        string data = await File.ReadAllTextAsync(path);
        return await _serializer.DeserializeMetadataAsync(data);
    }
    public async Task<List<LevelMetadata>> ReadLevelFileMetadata() {
        var levels = new List<LevelMetadata>();
        if (!Directory.Exists(LevelsFolder)) {
            Debug.LogWarning("Levels folder not found");
            return levels;
        }
        string[] files = Directory.GetFiles(LevelsFolder, $"*.{_serializer.FileExtension}", SearchOption.TopDirectoryOnly);
        var tasks = files.Select(file => ReadLevelFileMetadata(file));
        var results = await Task.WhenAll(tasks);
        
        return results.Where(x => x != null).ToList();
    }

    public async Task WriteLevelFile(Level level, string path) {
        var temp = path + ".tmp";

        var data = await _serializer.SerializeAsync(level);
        Debug.Log(data);
        // writes to temporary files to prevent crashes mid write
        await File.WriteAllTextAsync(temp, data);

        // replaces if file exists, else just "moves" the temp file to path
        if (File.Exists(path))
            File.Replace(temp, path, null); // windows only (i think)
        else
            File.Move(temp, path);
    }

    /// <summary>
    /// Overload of the WriteLevelFile which should be used when writing a LocalLevel,
    /// </summary>
    /// <param name="level"></param>
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
        if (string.IsNullOrEmpty(path)) return;
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
                audioPath = level.audioPath
            },
            Actions = level.actions
        };
    }
}