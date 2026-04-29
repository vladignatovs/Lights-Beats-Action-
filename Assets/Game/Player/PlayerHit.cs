using UnityEngine;

public class PlayerHit : MonoBehaviour {
    [Header ("Death SETTINGS ---------------")]
    public AttemptManager logic;
    public static bool playerIsAlive = true;

    [Header ("Dash SETTINGS ---------------")]
    public DashManager dashManager;
    public bool dashImmunity = false;
    public float cooldownBoost = 0;

    void Start() {
        playerIsAlive = true;
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<AttemptManager>();
        dashManager = GetComponent<DashManager>();
    }

    void Update() {
        if(!dashManager.onCooldown) {
            cooldownBoost = 1;
        }
    }

    void OnTriggerEnter2D(Collider2D collider2d) {
        if(dashImmunity) {

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

    void OnTriggerStay2D(Collider2D collider2d) {
        if(dashImmunity) {
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
