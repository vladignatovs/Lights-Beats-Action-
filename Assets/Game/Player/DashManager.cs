using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
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
        && !LogicManager.isPaused ) {
            target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            target.z = transform.position.z;
            StartCoroutine(dash());
            onCooldown = true;
            StartCoroutine(cooldown());
            StartCoroutine(fade(cooldownSeconds));
        }
    }

    /* ABANDONED IDEAS:
        | death animation (idea: graffiti explosion effect (not enough skill / will take away too much time & effort));
        | Visualizer as a concept (cannot be done with the build I have currently, and if can be, then will make 0 sense. Reimagined into action visualization);
        | Drag and drop action creation from a dropdown menu with icons (is not possible to combine UI elements and gameObjects like that (i think)).
    *//* FUTURE NOTES:
        Level completion should not be a basic "you won" screen as in gd, as in my opinion it will make the whole game look bland. 
    What I do want is to try to create some kind of unique animation. I think that it should be tightly connected to the overall
    UI design, as it should feel fitting and not soulless. Idk if its about time I try give this game its own charm or not, but I
    do know that I've had my own problems with same task in the past gd levels and projects of mine. I think I should mark out 
    the idea first, then try to improve it with time. Now that I'm looking at it, giving this game it's own vibe will make me redo
    all of the UI design i've done, and I'm not sure if that's what I'm aiming at with the level completion. I think the best option
    for now would be to make a simple and soulless end screen to keep on the progress going and not actually spend time brainstorming
    ideas for my UI. I'll figure things out as I go.
    *//* FEATURE IDEAS: 
        -- walls (Simillar to action, yet has a collision that player can stuck to. Could have a few types (dashable and undashable));
        -- dash Rs (dash resets, green floating orbs that act simillarly to dot attack, the difference being that it doesn't kill the player,
    yet gives a dash reset whenever player dashes through it);
        -- fog (?)(visibility reducing fog that dissipates whenever a player dashes beneath it). 
    *//* TODO IDEAS: 
        - revamp spawnpos; 
        - add an animation to progress bar when the level is finished, for example a flash or something else;
        - add ENUM options for MoveController;
        - song appearance at the start of the level;
        - fix dying in playtest mode accidentaly making you create a new action;
        - optimize editor loading. 
    *//* MOTIVATION CONTROL:
        [~16.08.24] seems like the motivation is still here, although i've started to allow myself more and more breaks, which
    is obviously not ideal. Still able to work on the project majority of my time though, so I'll continue to monitor.
        [26.08.24] became quickly irritated, the progress is going mad slow and I've started to get bored from 
    long sessions. Much worse than what I've had before. I should definitely spend some time rethinking the time I've spent as 
    I can't just throw all my work out the window like that.
    */

    // DONT FORGET TO WRITE UP EVERYTHING ON THE WAY, MAKE PLANS AND THINK THINGS THROUGH.
    // TODO: CONTINUE WORKING ON DATABASE INTEGRATION. Create new tables for each jsonfile (LevelSettings, LevelCompletions), 
    // find solutions for the data saved in them. Then integrate account scene into main menu and add more functionality to the accounts, 
    // like different rights, indepth account control. 
    
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

        // for(float i = 0; i < cooldownSeconds; i++) {
        //     // USEFUL, MIGHT WANT TO USE LATER
        //     // Debug.Log(i);
        //     while(time < 1) {
        //         Debug.Log(time);
        //         time += Time.deltaTime;
        //     }
        //     time = time - 1;
        //     yield return null;
        //     // yield return new WaitForSeconds(1f * playerHit.cooldownBoost);
        // }

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
