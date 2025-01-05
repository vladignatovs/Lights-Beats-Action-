using UnityEngine;
using UnityEngine.SceneManagement;

public class LogicManager : MonoBehaviour {
    public static bool isPaused = false;
    public GameObject gameOverScreen;
    public GameObject pauseMenu;

    public void restartGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        isPaused = false;
        Time.timeScale = 1;
    }

    public virtual void gameOver() {
        Time.timeScale = 0;
        isPaused = true;
        gameOverScreen.SetActive(true);
    }

    public void Resume() { 
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        isPaused = false;
    }

    public void Pause() {
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
        isPaused = true;
    }

    void Start() {
        Time.timeScale = 1;
        isPaused = false;
    }

    protected void Update() {
        if(!gameOverScreen.activeSelf && Input.GetKeyDown(KeyCode.Escape)) {
            if(pauseMenu.activeSelf) 
                Resume();
            else 
                Pause();
        }
    }
}
