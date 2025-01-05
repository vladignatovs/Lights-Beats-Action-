using System.Collections.Generic;
using System.IO;
using UnityEngine;
using LitJson;

public class LevelSettings {
    public int id;
    public List<(bool,bool,int)> actionsSettings; // each action has a specific position in the actionsList of a level

    public LevelSettings() {}
    public LevelSettings(int id) {
        this.id = id;
        actionsSettings = new();
    }
    public LevelSettings(int id, List<(bool,bool,int)> actionsSettings) {
        this.id = id;
        this.actionsSettings = new(actionsSettings);
    }
    /// <summary>
    /// Used to add a new levelSettings instance to the json file, should only be used when a level gets created.
    /// </summary>
    /// <param name="id"></param>
    public static void AddLevelSettings(int id) {
        string path = Application.dataPath + "/LevelSettings.json";
        JsonData levelSettingsJson = JsonMapper.ToObject(File.ReadAllText(path));
        LevelSettings lsToAdd = new(id);
        levelSettingsJson["LevelSettings"].Add(JsonMapper.ToObject(JsonMapper.ToJson(lsToAdd)));
        Debug.Log("LevelSettings added! Id: " + id);
        File.WriteAllText(path, levelSettingsJson.ToJson());
    }

    public static void DeleteLevelSettings(int id) {
        string path = Application.dataPath + "/LevelSettings.json";
        JsonData levelSettingsJson = JsonMapper.ToObject(File.ReadAllText(path));
        foreach(JsonData levelSettings in levelSettingsJson["LevelSettings"]) {
            int levelId = (int)levelSettings["id"];
            if(levelId == id) {
                levelSettingsJson["LevelSettings"].Remove(levelSettings);
                break;
            }
        }
        Debug.Log("LevelSettings removed! Id: " + id);
        File.WriteAllText(path, levelSettingsJson.ToJson());
    }

    public static void SaveLevelSettings(int id, List<(bool,bool,int)> actionsSettings) {
        string path = Application.dataPath + "/LevelSettings.json";
        JsonData levelSettingsJson = JsonMapper.ToObject(File.ReadAllText(path));
        foreach(JsonData levelSettings in levelSettingsJson["LevelSettings"]) {
            int levelId = (int)levelSettings["id"];
            if(levelId == id) {
                JsonData actionsSettingsToSave = new();
                actionsSettingsToSave.SetJsonType(JsonType.Array);
                foreach(var element in actionsSettings) {
                    actionsSettingsToSave.Add(JsonMapper.ToObject(JsonMapper.ToJson(element)));
                }
                levelSettings["actionsSettings"] = actionsSettingsToSave;
                break;
            }
        }
        Debug.Log("LevelSettings saved! Id: " + id);
        File.WriteAllText(path, levelSettingsJson.ToJson());
    }

    public static LevelSettings GetLevelSettings(int id) {
        string path = Application.dataPath + "/LevelSettings.json";
        JsonData levelSettingsJson = JsonMapper.ToObject(File.ReadAllText(path)); 
        foreach(JsonData levelSettings in levelSettingsJson["LevelSettings"]) {
            int levelId = (int)levelSettings["id"];
            if(levelId == id) {
                JsonData actionsSettingsJson = levelSettings["actionsSettings"];
                List<(bool,bool,int)> actionsSettings = new();
                foreach(JsonData element in actionsSettingsJson) {
                    bool item1 = (bool)element["Item1"];
                    bool item2 = (bool)element["Item2"];
                    int item3 = (int)element["Item3"];
                    
                    var actionValue = (item1, item2, item3);

                    actionsSettings.Add(actionValue);
                }
                return new LevelSettings(levelId, actionsSettings);
            }
        }
        return null;
    }
}