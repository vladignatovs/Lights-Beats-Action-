using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public readonly struct Scene {
    public string Name { get; }
    private Scene(string name) => Name = name;
    public static readonly Scene Main = new("Main");
    public static readonly Scene Game = new("Game");
    public static readonly Scene Editor = new("Editor");

    public static implicit operator string(Scene s) => s.Name;
}

/// <summary>
/// Static class managing all the scene state handling, a wrapper around SceneManager which 
/// allows for more explicit usage and less mistakes 
/// </summary>
public static class SceneStateManager {
    public static Scene PreviousScene { get; private set; } = Scene.Main;
    public static Scene CurrentScene { get; private set; } = Scene.Main;
    public static async Task Load(Scene scene) {
        PreviousScene = CurrentScene;
        await SceneManager.LoadSceneAsync(scene);
        CurrentScene = scene;
    }

    public static async Task Reload() => await Load(CurrentScene);
    public static async Task LoadMain() => await Load(Scene.Main);
    public static async Task LoadGame() => await Load(Scene.Game);
    public static async Task LoadEditor() => await Load(Scene.Editor);
    public static void Quit() => Application.Quit();
}