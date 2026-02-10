using UnityEngine;

public class ExplosionManager : MonoBehaviour {
    // ANIMATIONDURATION UNUSED
    [SerializeField] SpriteRenderer _spriteRenderer;
    [SerializeField] DurationsManager _durationsManager;
    [SerializeField] float _scaleFactor = 0.5f;
    float _lifeTime;
    Vector3 _initialScale;
    Color _visible;
    Color _invisible;
    // Update is called once per frame
    void Start() {
        _visible = _spriteRenderer.color;
        _invisible = new Color(_visible.r, _visible.g, _visible.b, 0);
        _initialScale = transform.localScale;
        _lifeTime = _durationsManager.getLifeTimeInSeconds();
    }
    void Update() {
        float timer = _durationsManager.Timer;
        transform.localScale = _initialScale * (1 + _scaleFactor * (timer/_lifeTime));
        _spriteRenderer.color = Color.Lerp(_visible, _invisible, timer/_lifeTime);
        if(timer > (_lifeTime/2)) {
            gameObject.GetComponent<Collider2D>().enabled = false;
            if(timer > _lifeTime)
                Destroy(gameObject);
        }
    }
}
