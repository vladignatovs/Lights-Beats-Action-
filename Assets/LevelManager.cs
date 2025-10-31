using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using UnityEngine;
public class LevelManager : MonoBehaviour {
    // single source of truth for the loaded levels
    public static List<Level> Levels { get; private set; }
    static string LevelsFolder => Path.Combine(Application.persistentDataPath, "Levels"); // TODO: make folder account specific

    static readonly JsonSerializerOptions Options = new() {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    static string GetLevelPath(int id) => Path.Combine(LevelsFolder, $"{id}.lvl");

    static int _lastId = 0;

    static LevelManager()
    {
        Options.Converters.Add(new FloatRoundingConverter());
    }

    public static async Task Initialize() {
        Levels = await LoadLevels();
        _lastId = (Levels != null && Levels.Count > 0) ? Levels.Max(l => l.localId) : 0;
    }

    public static int GetNextId() {
        _lastId++;
        return _lastId;
    }

    public static async Task CreateNewLevel(string name, float bpm, string audioPath) {
        Level level = new() {
            localId = GetNextId(),
            name = name,
            bpm = bpm,
            audioPath = audioPath,
            actions = new()
        };
        await SaveLevel(level);
    }

    public static async Task SaveLevel(Level level) {
        if (!Directory.Exists(LevelsFolder)) {
            Directory.CreateDirectory(LevelsFolder);
        }

        LevelFile levelFile = new() {
            Meta = new Meta {
                LocalId = level.localId,
                name = level.name,
                bpm = level.bpm,
                audioPath = level.audioPath
            },
            Actions = level.actions
        };

        var path = GetLevelPath(level.localId);
        var temp = path + ".tmp";

        var json = JsonSerializer.Serialize(levelFile, Options);
        Debug.Log(json);
        // writes to temporary files to prevent crashes mid write
        await File.WriteAllTextAsync(temp, json);

        // replaces if file exists, else just "moves" the temp file to path
        if (File.Exists(path))
            File.Replace(temp, path, null); // windows only (i think)
        else
            File.Move(temp, path);
    }

    public static async Task<Level> LoadLevel(int id) {
        var path = GetLevelPath(id);
        if (!File.Exists(path)) {
            Debug.Log("Doesnt exist: " + id);
            return null;
        }
        try {
            var json = await File.ReadAllTextAsync(path);
            var levelFile = JsonSerializer.Deserialize<LevelFile>(json, Options);

            return new Level {
                localId = levelFile.Meta.LocalId,
                name = levelFile.Meta.name,
                bpm = levelFile.Meta.bpm,
                audioPath = levelFile.Meta.audioPath,
                actions = levelFile.Actions
            };
        }
        catch (Exception ex) {
            Debug.LogError($"Failed to load level {id}: {ex.Message}");
            return null;
        }
    }

    public static void DeleteLevel(int id) {
        var path = GetLevelPath(id);
        if (File.Exists(path)) {
            File.Delete(path);
        }
    }

    static async Task<List<Level>> LoadLevels() {
        var levels = new List<Level>();

        if (!Directory.Exists(LevelsFolder)) {
            Debug.LogWarning("Levels folder not found");
            return levels;
        }

        string[] files = Directory.GetFiles(LevelsFolder, "*.lvl", SearchOption.TopDirectoryOnly);

        foreach (var file in files) {
            string json = await File.ReadAllTextAsync(file);
            var levelFile = JsonSerializer.Deserialize<LevelFile>(json, Options);
            if (levelFile != null) {
                levels.Add(new Level {
                    localId = levelFile.Meta.LocalId,
                    name = levelFile.Meta.name,
                    bpm = levelFile.Meta.bpm,
                    audioPath = levelFile.Meta.audioPath,
                    actions = levelFile.Actions
                });
            }
        }

        return levels;
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