using UnityEngine;
/// <summary>
/// The main class which sets the static data for a level to function in between scenes, provides a single
/// unified way of setting all the necessary data via passing a single level class instance
/// </summary>
public static class LevelInitializer {
    public static void InitializeLevel(Level level) {
        AudioClip audioClip = Resources.Load<AudioClip>("Audio/" + level.audioPath);

        StateNameManager.Level = level;
        StateNameManager.LoadedAudioClip = audioClip;
        
        float secondsPerBeat = 60f / level.bpm;
        StateNameManager.BeatAmount = (int)(audioClip.length / secondsPerBeat);
    }
}
