using UnityEngine;

public class TelegraphManager : MonoBehaviour {
    [SerializeField] SpriteRenderer _spriteRenderer;
    [SerializeField] DurationsManager _durationManager;
    float _duration;
    Color _visibleColor;
    Color _invisibleColor;

    void Start() {
        _invisibleColor = _spriteRenderer.color;
        _visibleColor = new Color(_invisibleColor.r, _invisibleColor.g, _invisibleColor.b, 1f);
        _duration = _durationManager.getAnimationDurationInSeconds();
    }
    void Update() {
        float timer = _durationManager.Timer;
        _spriteRenderer.color = Color.Lerp(_invisibleColor, _visibleColor, timer / _duration);
        if(timer >= _duration) {
            _spriteRenderer.color = Color.Lerp(_visibleColor, _invisibleColor, (timer - _duration) / .25f);
            if((timer - _duration) / .25f > 1) {
                Destroy(gameObject);
            }
        }
    }
}
