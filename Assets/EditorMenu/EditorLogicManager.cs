using UnityEngine;
public class EditorLogicManager : LogicManager {

    public EditorMenuManager editorMenuManager;
    [SerializeField] Camera _visualizerCamera;

    public override void gameOver() {
        editorMenuManager.TogglePlayTest(false); // false means that it should toggle playtest off.
    }

    public override void Pause() {
        base.Pause();
        Overlay.ToggleOverlay(_visualizerCamera, true);
    }
    public override void Resume() {
        base.Resume();
        Overlay.ToggleOverlay(_visualizerCamera, false);
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
