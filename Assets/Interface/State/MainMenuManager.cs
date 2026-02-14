using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

public enum MainMenuState {
    Default,
    Main,
    Official,
    Local,
    Server
}

/// <summary>
/// Class reliable for all the Main Menu animations and state management.
/// </summary>
public class MainMenuManager : MonoBehaviour {
    [SerializeField] Animator _mainMenuAnimator;
    [SerializeField] GameObject _background;

    public event System.Action<MainMenuState> OnStateChanged;

    async void Start() {
        // sets the state that was persisted before exit
        SetState(StateNameManager.LatestMainMenuState, true);
        // Time.timeScale = 1;
        // if was just in a game
        if(SceneStateManager.PreviousScene == Scene.Game) {
            _background.transform.position = StateNameManager.PlayerPosition;
            // play animation out of game
            GameTo();
            await MoveToCenter(0.75f);
            // TODO: figure out restarts with the animator states, currently hardcoded
        } else if (StateNameManager.LatestMainMenuState == MainMenuState.Local) {
            _mainMenuAnimator.Play("LocalLevelMenu");
        } else if (StateNameManager.LatestMainMenuState == MainMenuState.Server) {
            _mainMenuAnimator.Play("ServerLevelMenu");
        }
    }
    
    [UsedImplicitly]
    public void ToOfficial() => SetState(MainMenuState.Official);
    [UsedImplicitly]
    public void ToMain() => SetState(MainMenuState.Main);
    [UsedImplicitly]
    public void ToLocal() => SetState(MainMenuState.Local);
    [UsedImplicitly]
    public void ToServer() => SetState(MainMenuState.Server);

    // NOTE: "TO-GAME" animations must be played without state as it 
    // is later valuated to set the correct state for the animator
    public async Task ToGame() {
        var animation = StateNameManager.LatestMainMenuState switch {
            MainMenuState.Official => "OfficialToGame",
            MainMenuState.Local => "LocalToGame",
            MainMenuState.Server => "ServerToGame",
            _ => null
        };

        if (animation == null) return;
        _mainMenuAnimator.Play(animation);

        AnimatorStateInfo state;

        do {
            await Task.Yield();
            state = _mainMenuAnimator.GetCurrentAnimatorStateInfo(0);
        }
        while (!state.IsName(animation));

        await Task.Delay(Mathf.CeilToInt(state.length * 1000));
    }

    // NOTE: "GAME-TO" animations are played without state to avoid 
    // issues with animator evaluating default state faster
    void GameTo() {
        var animation = StateNameManager.LatestMainMenuState switch {
            MainMenuState.Official => "GameToOfficial",
            MainMenuState.Local => "GameToLocal",
            MainMenuState.Server => "GameToServer",
            _ => null,
        };

        if(animation == null) return;
        _mainMenuAnimator.Play(animation);
    }

    void SetState(MainMenuState newState, bool skipAnimation = false) {
        bool stateChanged = StateNameManager.LatestMainMenuState != newState;
        StateNameManager.LatestMainMenuState = newState;
        _mainMenuAnimator.SetInteger("MainMenuState", (int)newState); // Always sync animator
        // Only fire event if state actually changed
        if (stateChanged) OnStateChanged?.Invoke(newState);
        if (skipAnimation) _mainMenuAnimator.Update(0f);
    }

    async Task MoveToCenter(float duration) {
        Vector3 start = _background.transform.position;
        Vector3 target = Vector3.zero;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            _background.transform.position = Vector3.Lerp(start, target, t);
            await Task.Yield();
        }

        _background.transform.position = target;
    }
}