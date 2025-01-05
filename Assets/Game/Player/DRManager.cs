using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DRManager : MonoBehaviour
{
    public GameObject player;
    public bool isHovered = false;
    public SpriteRenderer sr;
    public Color defaultColor;
    // (26, 26, 37, 1)
    public Color onHoverColor;
    // (26, 40, 37, 1) OLD
    // (26, 26, 45, 1) NEW

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
        if(!LogicManager.isPaused && PlayerHit.playerIsAlive) {
            if(isHovered) {
                sr.color = onHoverColor;
            } else {
                sr.color = defaultColor;
            }
        } else {
            sr.color = defaultColor;
        }
    }

    void OnMouseEnter() {
        isHovered = true;
    }

    void OnMouseExit() {
        isHovered = false;
    }
}
