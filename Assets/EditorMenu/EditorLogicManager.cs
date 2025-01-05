using UnityEngine;
public class EditorLogicManager : LogicManager {

    public EditorMenuManager editorMenuManager;

    public override void gameOver() {
        editorMenuManager.TogglePlayTest(false); // false means that it should toggle playtest off.
    }

    new void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            if(pauseMenu.activeSelf) 
                Resume();
            else 
                Pause();
        }
    }
}
