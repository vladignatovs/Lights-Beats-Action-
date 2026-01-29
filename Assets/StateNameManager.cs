using UnityEngine;
public class StateNameManager {
    public static string LatestSceneName = "MainMenu";
    public static Level Level = new();
    public static float BeatAmount = 100;
    public static AudioClip LoadedAudioClip;
    public static Vector2 PlayerPosition = Vector2.zero;
    public static MainMenuState LatestMainMenuState;
}
