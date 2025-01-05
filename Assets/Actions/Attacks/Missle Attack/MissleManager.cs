using System.Threading;
using UnityEngine;

public class MissleManager : MonoBehaviour {
    // ANIMATIONDURATION UNUSED
    [Header("References")]
    [SerializeField] SpriteRenderer _spriteRenderer;
    [SerializeField] Rigidbody2D _rigidBody2d;
    [SerializeField] PlayerMovement _playerMovement;
    [SerializeField] DurationsManager _durationsManager;
    [SerializeField] GameObject _explosion;
    [Header("Parameters")]
    float _lifeTime;
    [SerializeField] float _speed = 10f;
    [SerializeField] float _acceleration = 35f;
    Vector3 _target;
    Color _curColor;
    // [SerializeField] private float lifeTime = 0.5f;
    void Start() {
        _curColor = _spriteRenderer.color;
        _playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        _lifeTime = _durationsManager.getLifeTimeInSeconds();
    }
    // Update is called once per frame
    void Update()
    {   
        _target = _playerMovement.transform.position;
        float angle = Mathf.Atan2(_target.y - transform.position.y, _target.x - transform.position.x) * Mathf.Rad2Deg -90f; //idk why its like that
        transform.eulerAngles = new Vector3(0, 0, Mathf.MoveTowardsAngle(transform.eulerAngles.z, angle, 160*Time.deltaTime));
        _speed += _acceleration * Time.deltaTime; // ACCELERATION
        _rigidBody2d.velocity = transform.up * _speed;
        float timer = _durationsManager.Timer;
        if(timer >= (_lifeTime / 4)) {
            _spriteRenderer.color = Color.Lerp(_curColor, Color.white, timer / (_lifeTime / 2));
            if(timer >= (_lifeTime/2))
                Explode();
                Destroy(gameObject);
        }
    }

    public void Explode() {
        GameObject mExplosion = Instantiate(_explosion, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
        mExplosion.transform.localScale = new Vector3(0.5f, 0.5f, 1);
        mExplosion.GetComponent<DurationsManager>().LifeTime = _lifeTime/2;
    }

    void OnTriggerEnter2D(Collider2D collider) {
        // PERHAPS UP TO CHANGE WITH FURTHER ADDITIONS
        if(!collider.CompareTag("Attack") && !collider.CompareTag("Border") && !collider.CompareTag("DashRadius")) {
            Explode();
            Destroy(gameObject);
        }
    }
}
