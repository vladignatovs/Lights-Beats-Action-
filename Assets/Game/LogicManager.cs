using UnityEngine;

public class LogicManager : MonoBehaviour {
    public GameObject gameOverScreen;

    // TODO: Proper DeathScreen handle
    void Start() {
        GameStateManager.IsGameOver = false;
        PauseManager.CanPause = true;
        Time.timeScale = 1;
    }

    public async void restartGame() {
        await SceneStateManager.Reload();
        GameStateManager.IsGameOver = false;
        PauseManager.CanPause = true;
        Time.timeScale = 1;
    }

    public virtual void gameOver() {
        Time.timeScale = 0;
        GameStateManager.IsGameOver = true;
        PauseManager.CanPause = false;
        gameOverScreen.SetActive(true);
    }
}
