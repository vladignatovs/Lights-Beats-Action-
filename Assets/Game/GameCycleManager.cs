using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameCycleManager : MonoBehaviour {
    [Header("Dependencies")]
    [SerializeField] AttemptManager _attemptManager;
    [SerializeField] LevelPauseManager _levelPauseManager;

    [Header("Death Screen")]
    [SerializeField] GameObject _deathScreen;

    [Header("Level Complete")]
    [SerializeField] AudioSource _audioSource;
    [SerializeField] GameObject _levelCompletePanel;
    [SerializeField] float _pitchFadeDuration = 1f;

    Text _levelCompleteRedirectText;
    bool _redirectCountdownStarted;
    bool _levelCompleteSequenceStarted;
    float _pitchStart;

    void Start() {
        _pitchStart = _audioSource.pitch;
        _levelCompleteRedirectText = _levelCompletePanel.GetComponentInChildren<Text>();
    }

    void OnEnable() {
        _attemptManager.OnDeathEvent += HandleDeath;
        _attemptManager.OnLevelCompleted += HandleLevelCompleted;
    }

    void OnDisable() {
        _attemptManager.OnDeathEvent -= HandleDeath;
        _attemptManager.OnLevelCompleted -= HandleLevelCompleted;
    }

    void Update() {
        if (_levelCompletePanel.activeSelf && !_redirectCountdownStarted) {
            _redirectCountdownStarted = true;
            PauseManager.CanPause = false;
            StartCoroutine(RedirectCountdown());
        }
    }

    void HandleDeath() {
        Overlay.ToggleOverlay(true);
        _deathScreen.SetActive(true);
    }

    void HandleLevelCompleted() {
        if (_levelCompleteSequenceStarted) return;
        _levelCompleteSequenceStarted = true;
        Time.timeScale = 0;
        PauseManager.CanPause = false;
        Overlay.ToggleOverlay(true);
        StartCoroutine(PlayLevelCompleteSequence());
    }

    IEnumerator PlayLevelCompleteSequence() {
        float timer = 0f;

        while (timer < _pitchFadeDuration) {
            timer += Time.unscaledDeltaTime;
            _audioSource.pitch = Mathf.Lerp(_pitchStart, 0f, timer / _pitchFadeDuration);
            yield return null;
        }

        _audioSource.pitch = 0f;
        _levelCompletePanel.SetActive(true);
    }

    IEnumerator RedirectCountdown() {
        for (int i = 3; i > 0; i--) {
            _levelCompleteRedirectText.text = "Redirecting in " + i + "...";
            yield return new WaitForSecondsRealtime(1);
        }
        _levelCompleteRedirectText.text = "bye";
        _levelPauseManager.GoToMenu();
        PauseManager.CanPause = true;
    }
}
