using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {
    public void GoToScene(string name) {
        var currentScene = SceneManager.GetActiveScene().name;

        StateNameManager.LatestSceneName = currentScene;
        if (currentScene == "Level") {
            var player = FindAnyObjectByType<PlayerMovement>();
            if (player != null) {
                StateNameManager.PlayerPosition = player.transform.position;
            }
            else {
                Debug.LogWarning("No PlayerMovement found in current level scene.");
            }
        }
        SceneManager.LoadScene(name);
    }

    public void GoToPreviousScene() {
        SceneManager.LoadScene(StateNameManager.LatestSceneName);
    }

    public void QuitGame() {
        Application.Quit();
        Debug.Log("Game Quit!");
    }
}
