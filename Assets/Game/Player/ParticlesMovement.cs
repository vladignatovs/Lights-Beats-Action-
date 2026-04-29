using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesMovement : MonoBehaviour
{
    public GameObject player;
    public ParticleSystem ps;
    public Animator playerAnimator;
    void Start()
    {
        playerAnimator = player.GetComponent<PlayerMovement>().animator;
    }

    void Update()
    {
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
        transform.rotation = player.transform.rotation;
        if(playerAnimator.GetBool("IsMoving") && !ps.isEmitting) {
            ps.Play();
        } else if(!playerAnimator.GetBool("IsMoving") && ps.isEmitting) {
            ps.Stop();
        }
    }
}
