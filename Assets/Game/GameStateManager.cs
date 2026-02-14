
public static class GameStateManager {
    public static bool IsEditorActive { get; private set; }
    public static bool IsGameOver { get; set; }
    public static bool IsRunning => !IsEditorActive && !PauseManager.IsPaused && !IsGameOver;
    public static void ToggleEditor(bool value) {
        IsEditorActive = value;
    }
}