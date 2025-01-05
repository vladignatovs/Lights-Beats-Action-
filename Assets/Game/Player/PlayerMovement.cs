using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    [Header ("Animation SETTINGS ---------------")]
    public Animator animator;
    private float previousHorizontal = 0f;
    private float previousVertical = 0f;

    [Header ("Movement SETTINGS ---------------")]
    public Rigidbody2D rb;
    public float speed = 12.5f;
    public float rotationSpeed = 720f;
    private float horizontal;
    private float vertical;
    private Vector2 movementDirection = Vector2.zero;
    public PlayerHit playerHit;

    [Header ("Hitbox SETTINGS ---------------")]

    public CircleCollider2D cc2d;
    public PolygonCollider2D pc2d;


    public bool isIdlePlaying() {
        return animator.GetCurrentAnimatorStateInfo(0).IsName("Idle");
    }

    // Start is called before the first frame update
    void Start()
    {
        playerHit = GetComponent<PlayerHit>();
        Vector3 target = transform.position;
    }

    // Update is called once per frame
    void Update()
    {   
        if (PlayerHit.playerIsAlive) {

            //In case of using controller, the animation will be different
            // and go to the default state later, making the game easier.
            animator.SetBool("IsMoving", false);

            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");

            if(movementDirection == Vector2.zero) {

                //IF PLAYER IS GOING UP && WAS NOT MOVING SIDEWAYS && WENT DOWN OR STAYED ON THE LAST FRAME BEFORE STOP
                if(vertical > 0) {
                    if(!isIdlePlaying() && previousHorizontal == 0 && !(previousVertical > 0)) {
                        transform.eulerAngles = Vector3.forward * 0;
                        animator.Play("Player_Transition");
                    } else if(isIdlePlaying()) {
                        transform.eulerAngles = Vector3.forward * 0;
                    } 

                //IF PLAYER IS GOING DOWN && WAS NOT MOVING SIDEWAYS && WENT UP OR STAYED ON THE LAST FRAME BEFORE STOP
                } else if(vertical < 0) {
                    if(!isIdlePlaying() && previousHorizontal == 0 && !(previousVertical < 0)) {
                        transform.eulerAngles = Vector3.forward * 180;
                        animator.Play("Player_Transition");
                    } else if(isIdlePlaying()) {
                        transform.eulerAngles = Vector3.forward * 180;
                    }

                //IF PLAYER IS GOING RIGHT && WAS NOT MOVING VERTICALLY && WENT LEFT OR STAYED ON THE LAST FRAME BEFORE STOP
                } else if(horizontal > 0) {
                    if(!isIdlePlaying() && previousVertical == 0 && !(previousHorizontal > 0)) {
                        transform.eulerAngles = Vector3.forward * -90;
                        animator.Play("Player_Transition");
                    } else if(isIdlePlaying()) {
                        transform.eulerAngles = Vector3.forward * -90;
                    }

                //IF PLAYER IS GOING LEFT && WAS NOT MOVING VERTICALLY && WENT RIGHT OR STAYED ON THE LAST FRAME BEFORE STOP
                } else if(horizontal < 0) {
                    if(!isIdlePlaying() && previousVertical == 0 && !(previousHorizontal < 0)) {
                        transform.eulerAngles = Vector3.forward * 90;
                        animator.Play("Player_Transition");
                    } else if (isIdlePlaying()) {   
                        transform.eulerAngles = Vector3.forward * 90;
                    }
                }
            }

            movementDirection = new Vector2(horizontal, vertical);
            float magnitude = Mathf.Clamp01(movementDirection.magnitude);
            movementDirection.Normalize();

            rb.velocity = movementDirection * speed * magnitude;
            // NOT CERTAIN OF THE CHANGES MADE, DO NOT ERASE THE COMMENT BELOW 
            // transform.Translate(movementDirection * speed * magnitude * Time.deltaTime, Space.World);

            if(movementDirection != Vector2.zero) {
                animator.SetBool("IsMoving", true);
                Quaternion toRotation = Quaternion.LookRotation(Vector3.forward, movementDirection);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
            }

            if(!Input.anyKey) {
                animator.SetBool("IsMoving", false);
            }

            // If not on the same spot, update the previous values (EVEN IF ONE OF THE VALUES IS 0)
            if(!(horizontal == 0f && vertical == 0f)) {
                previousHorizontal = horizontal;
                previousVertical = vertical;
            }
            // IMPORTANT, MIGHT WANT TO USE LATER
            // Debug.Log(previousHorizontal + " H : V " + previousVertical);

            if(animator.GetBool("IsMoving")) {
                cc2d.enabled = false;
                pc2d.enabled = true;
            } else {
                cc2d.enabled = true;
                pc2d.enabled = false;
            }
        }

    }
}
