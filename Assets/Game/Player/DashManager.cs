using System.Collections;
using UnityEngine;
public class DashManager : MonoBehaviour {   

    [Header ("DR SETTINGS ---------------")]
    public SpriteRenderer sr;
    private DRManager drManager;

    [Header("Player SETTINGS ---------------")]
    public Animator animator;
    public PlayerHit playerHit;
    public SpriteRenderer player_sr;
    public Color curColor;
    public Color cooldownColor;

    [Header ("Dash SETTINGS ---------------")]
    public float dashSpeed = 50f;
    private Vector3 target;
    public bool onCooldown = false;
    public float cooldownSeconds = 3f;

    void Start() {
        drManager = GameObject.FindGameObjectWithTag("DashRadius").GetComponent<DRManager>();
        playerHit = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHit>();
    }

    void Update() {
        if(Input.GetMouseButtonDown(0) 
        && drManager.isHovered 
        && PlayerHit.playerIsAlive
        && !onCooldown
        && GameStateManager.IsRunning ) {
            target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            target.z = transform.position.z;
            StartCoroutine(dash());
            onCooldown = true;
            StartCoroutine(cooldown());
            StartCoroutine(fade(cooldownSeconds));
        }
    }
    
    IEnumerator dash() {
        playerHit.dashImmunity = true;
        curColor = player_sr.color;
        player_sr.color = Color.white;
        animator.Play("Player_Dash_Active");
        float angle = Mathf.Atan2(target.y - transform.position.y, target.x - transform.position.x) * Mathf.Rad2Deg -90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        while(Vector3.Distance(transform.position, target) > 0.1f) {
            animator.SetBool("isDashing", true);
            transform.position = Vector3.MoveTowards(transform.position, target, dashSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0, 0, angle);
            yield return null;
        }
        animator.SetBool("isDashing", false);
        yield return new WaitForSeconds(0.1f);
        player_sr.color = curColor;
        playerHit.dashImmunity = false;
    }

    IEnumerator cooldown() {
        float time = 0;
        while(time < cooldownSeconds * playerHit.cooldownBoost) {
            time += Time.deltaTime;
            yield return null;
        }
        onCooldown = false;
    }

    IEnumerator fade(float duration) {
        Color visible = drManager.defaultColor;
        Color notVisible = new Color(visible.r, visible.g, visible.b, 0);
        float time = 0f;
        while(time < duration * playerHit.cooldownBoost) {
            if(player_sr.color != Color.white) {
                player_sr.color = Color.Lerp(curColor, cooldownColor, Mathf.PingPong(Time.time * 12.5f, 1));
            }
            time += Time.deltaTime;
            sr.color = Color.Lerp(notVisible, visible, time / (duration * playerHit.cooldownBoost));
            yield return null;
        }
        player_sr.color = curColor;
    }
}
