using UnityEngine;

public class EditorMenuManager : MonoBehaviour {
    public GameObject editorMenu;
    public GameObject pauseMenu;
    public EditorBeatManager editorBeatManager;
    public GameObject audioLine;
    public GameObject player;
    public GameObject dashRadius;
    public Camera mainCamera;
    public Camera visualiserCamera;
    public GameObject actionCreator;
    GameObject activePlayer;
    GameObject activeDR;
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0;
    }

    // Update is called once per frame
    void Update() {
        if(!pauseMenu.activeSelf && Input.GetKeyDown(KeyCode.Tab)) {
            TogglePlayTest(editorMenu.activeSelf);
        }
    }

    public void TogglePlayTest(bool value) {
        if(value) {
            PlayTest();
        } else {
            EndPlayTest();
        }  
    }
    void PlayTest() {
        mainCamera.enabled = true;
        visualiserCamera.enabled = false;

        audioLine.SetActive(false);

        LogicManager.isPaused = false;
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

        LogicManager.isPaused = true;
        Time.timeScale = 0;
        editorBeatManager.enabled = false;
        editorMenu.SetActive(true);

        actionCreator.GetComponent<ActionCreator>().DeselectAllActions();

        Destroy(activePlayer);
        Destroy(activeDR);
    }
}
