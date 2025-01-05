using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LevelCompleteManager : MonoBehaviour {
    [Header("Level-Complete-Panel Management")]
    public AudioSource audioSource;
    public GameObject levelCompletePanel;
    public MenuManager menuManager;
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
            StartCoroutine(RedirectCountdown());
        }
    }
    
    IEnumerator RedirectCountdown() {
        for(int i = 3; i > 0; i--) {
            levelCompleteReidrectText.text = "Redirecting in " + i + "...";
            yield return new WaitForSeconds(1);
        }
        levelCompleteReidrectText.text = "bye";
        menuManager.GoToScene("LevelSelect");
    }
    
    public void LevelComplete(int id) {
        LevelCompletionsManager.CompleteLevel(id);
        timer += Time.deltaTime;
        audioSource.pitch = Mathf.Lerp(pitchStart, 0, timer / duration);
        if(timer >= duration) {
            levelCompletePanel.SetActive(true);
        }
    }
}