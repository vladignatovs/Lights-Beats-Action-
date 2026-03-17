using UnityEngine;

public class DRManager : MonoBehaviour {
    public float HoveredTimeSeconds { get; private set; }
    public GameObject player;
    public bool isHovered = false;
    public SpriteRenderer sr;
    public Color defaultColor;
    // (26, 26, 37, 1)
    public Color onHoverColor;
    // (26, 40, 37, 1) OLD
    // (26, 26, 45, 1) NEW

    // Update is called once per frame
    void Update() {
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
        if(GameStateManager.IsRunning && PlayerHit.playerIsAlive) {
            if(isHovered) {
                sr.color = onHoverColor;
                HoveredTimeSeconds += Time.deltaTime;
            } else {
                sr.color = defaultColor;
            }
        } else {
            sr.color = defaultColor;
        }
    }

    void OnMouseEnter() => isHovered = true;
    void OnMouseExit() => isHovered = false;    
    public void ResetHoveredTime() => HoveredTimeSeconds = 0f;
}
