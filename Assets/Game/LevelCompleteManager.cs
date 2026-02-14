using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LevelCompleteManager : MonoBehaviour {
    [Header("Level-Complete-Panel Management")]
    public AudioSource audioSource;
    public GameObject levelCompletePanel;
    public LevelPauseManager _levelPauseManager;
    Text levelCompleteReidrectText;
    bool redirectCountdownStarted = false;

    [Header("Level-Complete-Audio Management")]
    [SerializeField] float duration = 1;
    float pitchStart;
    float timer;
    // Start is called before the first frame update
    void Start() {
        pitchStart = audioSource.pitch;
        levelCompleteReidrectText = levelCompletePanel.GetComponentInChildren<Text>();
    }

    // Update is called once per frame
    void Update() {
        if(levelCompletePanel.activeSelf && !redirectCountdownStarted) {
            redirectCountdownStarted = true;
            PauseManager.CanPause = false;
            StartCoroutine(RedirectCountdown());
        }
    }
    
    IEnumerator RedirectCountdown() {
        for(int i = 3; i > 0; i--) {
            levelCompleteReidrectText.text = "Redirecting in " + i + "...";
            yield return new WaitForSeconds(1);
        }
        levelCompleteReidrectText.text = "bye";
        _levelPauseManager.GoToMenu();
        PauseManager.CanPause = true;
    }
    
    public void LevelComplete(int id) {
        // complete server and official levels via data manager
        // LevelCompletionsManager.CompleteLevel(id);
        timer += Time.deltaTime;
        audioSource.pitch = Mathf.Lerp(pitchStart, 0, timer / duration);
        if(timer >= duration) {
            levelCompletePanel.SetActive(true);
        }
    }
}