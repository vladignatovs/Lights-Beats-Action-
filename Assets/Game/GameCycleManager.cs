using System.Collections;
using UnityEngine;

public class GameCycleManager : MonoBehaviour {
    [Header("Dependencies")]
    [SerializeField] AttemptManager _attemptManager;
    [SerializeField] DashManager _dashManager;
    [SerializeField] ParticlesMovement _particlesMovement;

    [Header("Death Screen")]
    [SerializeField] GameObject _deathScreen;

    [Header("Level Complete")]
    [SerializeField] AudioSource _audioSource;
    [SerializeField] GameObject _levelCompletePanel;
    [SerializeField] float _pitchFadeDuration = 1f;

    bool _levelCompleteSequenceStarted;
    float _pitchStart;

    void Start() {
        _pitchStart = _audioSource.pitch;
    }

    void OnEnable() {
        _attemptManager.OnDeathEvent += HandleDeath;
        _attemptManager.OnLevelCompleted += HandleLevelCompleted;
    }

    void OnDisable() {
        _attemptManager.OnDeathEvent -= HandleDeath;
        _attemptManager.OnLevelCompleted -= HandleLevelCompleted;
    }

    void HandleDeath() {
        Overlay.ToggleOverlay(true);
        _deathScreen.SetActive(true);
    }

    void HandleLevelCompleted() {
        if (_levelCompleteSequenceStarted) return;
        _levelCompleteSequenceStarted = true;
        PauseManager.CanPause = false;
        Overlay.ToggleOverlay(true);
        StartCoroutine(PlayLevelCompleteSequence());
    }

    IEnumerator PlayLevelCompleteSequence() {
        float timer = 0f;
        float startTimeScale = Time.timeScale;
        Vector3 playerStartScale = _dashManager.transform.localScale;
        Vector3 dashRadiusStartScale = _dashManager.sr.transform.localScale;
        Vector3 particlesStartScale = _particlesMovement.transform.localScale;

        while (timer < _pitchFadeDuration) {
            timer += Time.unscaledDeltaTime;
            float progress = timer / _pitchFadeDuration;

            Time.timeScale = Mathf.Lerp(startTimeScale, 0f, progress);
            _audioSource.pitch = Mathf.Lerp(_pitchStart, 0f, progress);

            _dashManager.transform.localScale = Vector3.Lerp(playerStartScale, Vector3.zero, progress);
            _dashManager.sr.transform.localScale = Vector3.Lerp(dashRadiusStartScale, Vector3.zero, progress);
            _particlesMovement.transform.localScale = Vector3.Lerp(particlesStartScale, Vector3.zero, progress);

            yield return null;
        }

        Time.timeScale = 0f;
        _audioSource.pitch = 0f;
        _dashManager.transform.localScale = Vector3.zero;
        _dashManager.sr.transform.localScale = Vector3.zero;
        _particlesMovement.transform.localScale = Vector3.zero;

        _levelCompletePanel.SetActive(true);
    }
}
