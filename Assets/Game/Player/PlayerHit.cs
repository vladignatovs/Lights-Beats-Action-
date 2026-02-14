using UnityEngine;

public class PlayerHit : MonoBehaviour {
    [Header ("Death SETTINGS ---------------")]
    public LogicManager logic;
    public static bool playerIsAlive = true;

    [Header ("Dash SETTINGS ---------------")]
    public DashManager dashManager;
    public bool dashImmunity = false;
    public float cooldownBoost = 0;

    void Start() {
        playerIsAlive = true;
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicManager>();
        dashManager = GetComponent<DashManager>();
    }

    void Update() {
        if(!dashManager.onCooldown) {
            cooldownBoost = 1;
        }
    }

    void OnTriggerEnter2D(Collider2D collider2d) {
        if(dashImmunity) {

            // Really dumb way of solving it, UP TO CHANGE !!!!!!!!!!!!!!!!!!!!!!!!!!
            if(!collider2d.gameObject.CompareTag("Attack")) {
                return;
            }
            
            cooldownBoost = 0.1f;
            return;
        }

        if(collider2d.gameObject.CompareTag("Attack")) {
            logic.gameOver();
            playerIsAlive = false;
        }

        // Physics2D.IgnoreLayerCollision(3, 6, true);
    }

    void OnTriggerStay2D(Collider2D collider2d) {
        if(dashImmunity) {

            // Really dumb way of solving it, UP TO CHANGE !!!!!!!!!!!!!!!!!!!!!!!!!!
            if(!collider2d.gameObject.CompareTag("Attack")) {
                return;
            }
            
            cooldownBoost = 0.1f;
            return;
        }

        if(collider2d.gameObject.CompareTag("Attack")) {
            logic.gameOver();
            playerIsAlive = false;
        }
    }
}
