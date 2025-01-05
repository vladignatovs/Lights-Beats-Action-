using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using LitJson;
using UnityEngine.UI;
using System.Text;

public class LevelManager : MonoBehaviour {

    /// <summary>
    /// Point of this method is to create a new level in the database, and it doesn't parse in the id of the level, as it doesn't exist in
    /// the database yet. When creating a new level, it will most definitely have an empty actions list, so I didn't even bother fixing it.
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public static IEnumerator CreateLevel(Level level) {
        WWWForm form = new();
        form.AddField("level_name", level.levelName);
        form.AddField("bpm", level.bpm.ToString());
        form.AddField("audio_path", level.audioPath);

        UnityWebRequest wReq = UnityWebRequest.Post("http://localhost/sqlconnect/create_level.php", form);
        yield return wReq.SendWebRequest();

        if(wReq.result == UnityWebRequest.Result.Success) {
            if(wReq.downloadHandler.text == "success") {
                Debug.Log("Level created successfully");
            } else {
                Debug.LogError("Failed to created level: " + wReq.downloadHandler.text);
            }
        } else {
            Debug.LogError("Failed to created level: " + wReq.error);
        }
    }

    // public void CallUpdateLevel(Level level) {
    //     StartCoroutine(UpdateLevel(level));
    // }

    /// <summary>
    /// Updating a level is a complicated operation as it might not change some of the aspects of the level. Updating all of the levels
    /// values at once might be unoptimzied and inefficient, so should be avoided. In order to do so I should check what information is
    /// being changed.
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public static IEnumerator UpdateLevel(Level level) {
        WWWForm form = new();
        form.AddField("id", level.id.ToString());
        form.AddField("level_name", level.levelName);

        StringBuilder actionsBuilder = new ();
        actionsBuilder.Append("[");
        foreach (var action in level.actions) {
            actionsBuilder.Append(JsonUtility.ToJson(action)).Append(",");
        }
        if (actionsBuilder.Length > 1) {
            actionsBuilder.Length--; // Remove the trailing comma
        }
        actionsBuilder.Append("]");
        form.AddField("actions", actionsBuilder.ToString());

        form.AddField("bpm", level.bpm.ToString());
        form.AddField("audio_path", level.audioPath);

        Debug.Log(actionsBuilder.ToString());
        UnityWebRequest wReq = UnityWebRequest.Post("http://localhost/sqlconnect/update_level.php", form);
        yield return wReq.SendWebRequest();

        if(wReq.result == UnityWebRequest.Result.Success) {
            if(wReq.downloadHandler.text == "success") {
                Debug.Log("Level updated successfully");
            } else {
                Debug.LogError("Failed to update level: " + wReq.downloadHandler.text);
            }
        } else {
            Debug.LogError("Failed to update level: " + wReq.error);
        }
    }

    public static IEnumerator GetLevels(Action<List<Level>> callback) {
        UnityWebRequest wReq = UnityWebRequest.Get("http://localhost/sqlconnect/get_levels.php");
        yield return wReq.SendWebRequest();

        if(wReq.result != UnityWebRequest.Result.Success) {
            Debug.LogError("Failed to get levels: " + wReq.error);
        }
        JsonData jsonData = JsonMapper.ToObject(wReq.downloadHandler.text);
        List<Level> levels = new();
        foreach(JsonData level in jsonData) {
            Level newLevel = new() {
                id = (int)level["id"],
                levelName = level["level_name"].ToString(),
                audioPath = level["audio_path"].ToString()
            };
            levels.Add(newLevel);
        }
        callback(levels);
    }

    public static IEnumerator GetLevelById(Action<Level> callback) {
        WWWForm form = new();
        form.AddField("id", StateNameManager.Level.id);
        UnityWebRequest wReq = UnityWebRequest.Post("http://localhost/sqlconnect/get_level_by_id.php", form);
        yield return wReq.SendWebRequest();
        if(wReq.result == UnityWebRequest.Result.Success) {
            Debug.Log("Level gotten successfully.");
        } else {
            Debug.LogError("Failed to get level by ID: " + wReq.error);
        }

        JsonData jsonData = JsonMapper.ToObject(wReq.downloadHandler.text);
        Level newLevel = new() {
            id = (int)jsonData["id"],
            levelName = jsonData["level_name"].ToString(),
            actions = new List<Action>(),
            bpm = (float)jsonData["bpm"],
            audioPath = jsonData["audio_path"].ToString()
        };

        if(jsonData["actions"] != null) {
            newLevel.actions = JsonMapper.ToObject<List<Action>>(jsonData["actions"].ToString());
        }

        callback(newLevel);
    }

    public static IEnumerator DeleteLevel() {
        WWWForm form = new();
        form.AddField("id", StateNameManager.Level.id);
        UnityWebRequest wReq = UnityWebRequest.Post("http://localhost/sqlconnect/delete_level_by_id.php", form);

        yield return wReq.SendWebRequest();

        if(wReq.result == UnityWebRequest.Result.Success) {
            if(wReq.downloadHandler.text == "success") 
                Debug.Log("Level deleted successfully.");
            else
                Debug.LogError("Failed to delete level: " + wReq.downloadHandler.text);
        } else {
            Debug.LogError("Failed to delete: " + wReq.error);
        }
    }
}