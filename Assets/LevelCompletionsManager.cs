using System.IO;
using UnityEngine;
using LitJson;

public class LevelCompletionsManager : MonoBehaviour {
/* NOTES: 
    This class doesn't guarantee the uniqueness of the ids it is taking;
    Make sure all of the use cases are connected with existing level objects. 
*/ 
    // Called at the same time as Level.SaveLevel method, takes in the id from the saved level.
    public static void AddLevelCompletion(int id) {
        string path = Application.dataPath + "/LevelCompletions.json";
        JsonData levelCompletionsJson = JsonMapper.ToObject(File.ReadAllText(path));
        LevelCompletion levelCompletionToAdd = new() {
            id = id,
            completed = false
        };
        levelCompletionsJson["LevelCompletions"].Add(JsonMapper.ToObject(JsonMapper.ToJson(levelCompletionToAdd)));
        Debug.Log("LevelCompletion added successfully! Id: " + id);
        File.WriteAllText(path, levelCompletionsJson.ToJson());
    }
    // Called in the LevelCompleteManager, completes the level as soon as levelComplete method is called.
    public static void CompleteLevel(int id) {
        string path = Application.dataPath + "/LevelCompletions.json";
        JsonData levelCompletionsJson = JsonMapper.ToObject(File.ReadAllText(path));
        foreach(JsonData levelCompletion in levelCompletionsJson["LevelCompletions"]) {
            int levelId = (int)levelCompletion["id"];
            if(levelId == id) {
                levelCompletion["completed"] = true;
                break;
            }
        }
        Debug.Log("Level Completed! Id: " + id);
        File.WriteAllText(path, levelCompletionsJson.ToJson());
    }

    // Called in the actionCreator deleteLevel method, is called at the same time as Level.DeleteLevel, takes in the same id from actionCreator.
    public static void DeleteLevelCompletion(int id) {
        string path = Application.dataPath + "/LevelCompletions.json";
        JsonData levelCompletionsJson = JsonMapper.ToObject(File.ReadAllText(path));
        foreach (JsonData levelCompletion in levelCompletionsJson["LevelCompletions"]) {
            int levelId = (int)levelCompletion["id"];
            if (levelId == id) {
                levelCompletionsJson["LevelCompletions"].Remove(levelCompletion);
                break;
            }
        }
        Debug.Log("Level Completion deleted! Id: " + id);
        File.WriteAllText(path, levelCompletionsJson.ToJson());
    }

    public static bool GetCompletionFromId(int id) {
        string path = Application.dataPath + "/LevelCompletions.json";
        JsonData levelCompletionsJson = JsonMapper.ToObject(File.ReadAllText(path));
        foreach (JsonData levelCompletion in levelCompletionsJson["LevelCompletions"]) {
            int levelId = (int)levelCompletion["id"];
            if (levelId == id) {
                return (bool)levelCompletion["completed"];
            }
        }
        Debug.Log("Level Completion not found! Id: " + id);
        return false;
    }
}

public class LevelCompletion { // A class used to pretty much just simplify adding of levelCompletions.
    public int id;
    public bool completed;
    public LevelCompletion() {}
    public LevelCompletion(int id, bool completed) {
        this.id = id;
        this.completed = completed;
    }
}