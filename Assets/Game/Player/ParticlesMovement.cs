using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesMovement : MonoBehaviour
{
    public GameObject player;
    public ParticleSystem ps;
    public Animator playerAnimator;
    // Start is called before the first frame update
    void Start()
    {
        playerAnimator = player.GetComponent<PlayerMovement>().animator;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
        transform.rotation = player.transform.rotation;
        // Debug.Log(ps.isEmitting);
        if(playerAnimator.GetBool("IsMoving") && !ps.isEmitting) {
            ps.Play();
        } else if(!playerAnimator.GetBool("IsMoving") && ps.isEmitting) {
            ps.Stop();
        }
    }
}
