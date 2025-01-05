using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {
    public void GoToScene(string name) {
        StateNameManager.LatestSceneName = SceneManager.GetActiveScene().name;
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
