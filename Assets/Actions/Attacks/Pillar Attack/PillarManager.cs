using UnityEngine;

public class PillarMovement : MonoBehaviour {
    [Header ("Actions")]
    [SerializeField] DurationsManager _durationsManager;
    float _duration;
    float _lifeTime;
    
    [Header ("Visuals")]
    [SerializeField] SpriteRenderer _spriteRenderer;
    [SerializeField] Color _curColor;
    Color _invisibleColor;
    Vector3 _curScale;
    Vector3 _scaledDown;
    void Start() {
        _lifeTime = _durationsManager.getLifeTimeInSeconds();
        _duration = _durationsManager.getAnimationDurationInSeconds();
        _curScale = transform.localScale;
        _scaledDown = new Vector3(0, _curScale.y, _curScale.z);
        _invisibleColor = _curColor;
        _invisibleColor.a = 0;
    }
    void Update() {
        //Counts the timer, and changes the color from 0 to 1, 
        // which would happen in a single second, however since 
        // it is also divided by the duration, the effect is either
        // sped up or slowed down.(0,1/0,25=0,4) ~ 4 times faster, or 0.25 sec.
        float timer = _durationsManager.Timer;
        _spriteRenderer.color = Color.Lerp(Color.white, _curColor, timer/_duration);
        //Added lifeTime, not decreasing or adding duration to the timer, as that
        // makes the first spawn animation be a part of overall lifeTime
        if(timer >= (_lifeTime-_duration)) {
            transform.localScale = Vector3.Lerp(_curScale, _scaledDown, (timer - (_lifeTime - _duration)) / _duration);
            _spriteRenderer.color = Color.Lerp(_curColor, _invisibleColor, (timer - (_lifeTime - _duration)) / _duration);
        } else {
            transform.localScale = new Vector3(_curScale.x + Mathf.PingPong(Time.time * 100,0.1f) - 0.05f, transform.localScale.y, 1);
        }
        if(timer > _lifeTime)
            Destroy(gameObject);
    }
}
