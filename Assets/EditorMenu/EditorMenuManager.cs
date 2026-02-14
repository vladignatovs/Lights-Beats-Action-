using UnityEngine;

public class EditorMenuManager : MonoBehaviour {
    public GameObject editorMenu;
    [SerializeField] EditorBeatManager editorBeatManager;
    public GameObject audioLine;
    public GameObject player;
    public GameObject dashRadius;
    public Camera mainCamera;
    public Camera visualiserCamera;
    public GameObject actionCreator;
    GameObject activePlayer;
    GameObject activeDR;
 
    void Start() {
        GameStateManager.ToggleEditor(true);
        Time.timeScale = 0;
    }

    void Update() {
        if(!PauseManager.IsPaused && Input.GetKeyDown(KeyCode.Tab)) {
            TogglePlayTest(editorMenu.activeSelf);
        }
    }

    // handle exiting editor
    void OnDestroy() {
        GameStateManager.ToggleEditor(false);
        Time.timeScale = 1;
    }

    public void TogglePlayTest(bool value) {
        if(value) PlayTest();
        else EndPlayTest();
    }

    void PlayTest() {
        mainCamera.enabled = true;
        visualiserCamera.enabled = false;

        audioLine.SetActive(false);

        GameStateManager.ToggleEditor(false);
        Time.timeScale = 1;
        editorBeatManager.enabled = true;
        editorMenu.SetActive(false);

        activePlayer = Instantiate(player);
        activeDR = Instantiate(dashRadius);
        activePlayer.GetComponent<DashManager>().sr = activeDR.GetComponent<SpriteRenderer>();
        activeDR.GetComponent<DRManager>().player = activePlayer;
    }

    void EndPlayTest() {
        // ORDER MATTERS
        mainCamera.enabled = false;
        visualiserCamera.enabled = true;

        GameStateManager.ToggleEditor(true);
        Time.timeScale = 0;
        editorBeatManager.enabled = false;
        editorMenu.SetActive(true);

        actionCreator.GetComponent<ActionCreator>().DeselectAllActions();

        Destroy(activePlayer);
        Destroy(activeDR);
    }
}
