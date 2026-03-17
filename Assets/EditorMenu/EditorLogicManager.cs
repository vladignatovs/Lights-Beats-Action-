public class EditorLogicManager : AttemptManager {
    public EditorMenuManager editorMenuManager;

    protected override bool UsesGameplayStats => false;

    public override float AccuracyPercent => 0f;
    public override float CompletionPercent => 0f;

    // In editor death screen should instead simply turn off playtest
    public override void gameOver() {
        editorMenuManager.TogglePlayTest(false);
    }
}
