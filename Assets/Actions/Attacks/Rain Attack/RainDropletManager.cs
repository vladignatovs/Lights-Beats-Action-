using UnityEngine;

public class RainDropletManager : MonoBehaviour {
    [SerializeField] DurationsManager _durationsManager;
    [SerializeField] SpriteRenderer _spriteRenderer;
    float _duration;
    float _lifeTime;
    Color _curColor;
    Color _invisibleColor;
    // Start is called before the first frame update
    void Start() {
        _curColor = _spriteRenderer.color;
        _invisibleColor = _curColor;
        _invisibleColor.a = 0;

        _duration = _durationsManager.getAnimationDurationInSeconds();
        _lifeTime = _durationsManager.getLifeTimeInSeconds();
    }

    // Update is called once per frame
    void Update() {   
        float timer = _durationsManager.Timer;
        _spriteRenderer.color = Color.Lerp(Color.white, _curColor, timer / _duration);

        if(timer > (_lifeTime - _duration)) {
            gameObject.GetComponent<Collider2D>().enabled = false;
            _spriteRenderer.color = Color.Lerp(_curColor, _invisibleColor, (timer - (_lifeTime - _duration)) / _duration);
            if(timer > _lifeTime)
                Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collider) {
        if(collider.transform.localScale.y > 24) {
            Destroy(gameObject);
        }
    }
}
