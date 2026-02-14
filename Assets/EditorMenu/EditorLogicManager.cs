public class EditorLogicManager : LogicManager {
    public EditorMenuManager editorMenuManager;

    // In editor death screen should instead simply turn off playtest
    public override void gameOver() {
        editorMenuManager.TogglePlayTest(false);
    }
}
