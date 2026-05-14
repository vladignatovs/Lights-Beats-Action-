using System.Collections;
using UnityEngine;

public class GameCycleManager : MonoBehaviour {
    [Header("Dependencies")]
    [SerializeField] AttemptManager _attemptManager;
    [SerializeField] DashManager _dashManager;
    [SerializeField] ParticlesMovement _particlesMovement;

    [Header("Level Complete")]
    [SerializeField] AudioSource _audioSource;
    [SerializeField] GameObject _levelCompletePanel;

    bool _levelCompleteSequenceStarted;
    bool _deathSequenceStarted;

    void OnEnable() {
        _attemptManager.OnDeathEvent += HandleDeath;
        _attemptManager.OnLevelCompleted += HandleLevelCompleted;
    }

    void OnDisable() {
        _attemptManager.OnDeathEvent -= HandleDeath;
        _attemptManager.OnLevelCompleted -= HandleLevelCompleted;
    }

    void HandleDeath() {
        if (_deathSequenceStarted) return;
        _deathSequenceStarted = true;
        PlayerHit.playerIsAlive = false;
        StartCoroutine(PlayDeathSequence());
    }

    IEnumerator PlayDeathSequence() {
        ThrowDeathParticles();
        yield return PlaySlowAndScaleSequence(scaleDuration: 0.2f, pitchDuration: 0.6f);

        _attemptManager.restartGame();
    }

    void ThrowDeathParticles() {
        ParticleSystem ps = _particlesMovement.ps;
        ParticleSystem.MainModule main = ps.main;
        main.useUnscaledTime = true;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        _particlesMovement.enabled = false;
        _particlesMovement.transform.position = _dashManager.transform.position;
        ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);

        for (int i = 0; i < 24; i++) {
            float angle = 360f / 24 * i;
            Vector2 direction = new(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

            ParticleSystem.EmitParams emitParams = new() {
                position = _dashManager.transform.position,
                velocity = direction * 8f
            };

            ps.Emit(emitParams, 1);
        }
    }

    void HandleLevelCompleted() {
        if (_levelCompleteSequenceStarted) return;
        _levelCompleteSequenceStarted = true;
        PauseManager.CanPause = false;
        Overlay.ToggleOverlay(true);
        StartCoroutine(PlayLevelCompleteSequence());
    }

    IEnumerator PlayLevelCompleteSequence() {
        yield return PlaySlowAndScaleSequence(slowTime: true, scaleParticles: true);

        _levelCompletePanel.SetActive(true);
    }

    IEnumerator PlaySlowAndScaleSequence(float duration = 1f, float scaleDuration = 1f, float pitchDuration = 1f, bool slowTime = false, bool scaleParticles = false) {
        float timer = 0f;
        float startTimeScale = Time.timeScale;
        float pitchStart = _audioSource.pitch;
        Vector3 playerStartScale = _dashManager.transform.localScale;
        Vector3 dashRadiusStartScale = _dashManager.sr.transform.localScale;
        Vector3 particlesStartScale = _particlesMovement.transform.localScale;

        while (timer < duration) {
            timer += Time.unscaledDeltaTime;
            float scaleProgress = Mathf.Clamp01(timer / scaleDuration);
            float pitchProgress = Mathf.Clamp01(timer / pitchDuration);
            float timeProgress = Mathf.Clamp01(timer / duration);

            if (slowTime) Time.timeScale = Mathf.Lerp(startTimeScale, 0f, timeProgress);
            _audioSource.pitch = Mathf.Lerp(pitchStart, 0f, pitchProgress);

            _dashManager.transform.localScale = Vector3.Lerp(playerStartScale, Vector3.zero, scaleProgress);
            _dashManager.sr.transform.localScale = Vector3.Lerp(dashRadiusStartScale, Vector3.zero, scaleProgress);
            if (scaleParticles) _particlesMovement.transform.localScale = Vector3.Lerp(particlesStartScale, Vector3.zero, scaleProgress);

            yield return null;
        }

        if (slowTime) Time.timeScale = 0f;
        _audioSource.pitch = 0f;
        _dashManager.transform.localScale = Vector3.zero;
        _dashManager.sr.transform.localScale = Vector3.zero;
        if (scaleParticles) _particlesMovement.transform.localScale = Vector3.zero;
    }
}
