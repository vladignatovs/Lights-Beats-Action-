using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMovement : MonoBehaviour
{
    public Rigidbody2D rigidBody2d;
    public float speed = 10f;

    public float acceleration = 0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!(transform.position.y < 11)) {
            Destroy(gameObject);
        }

    }

    void FixedUpdate() {
        speed += acceleration;
        rigidBody2d.velocity = new Vector2(rigidBody2d.velocity.x, speed);
        acceleration++;
    }
}
