using UnityEngine;
public class EditorBeatManager : BasicBeatManager {
    [Header("                UNIQUE EDITOR REFS")]
    [SerializeField] ActionCreator _actionCreator;
    [SerializeField] AudioLineManager _audioLineManager;
    [SerializeField] EditorMenuManager _editorMenuManager;
    [SerializeField] SpawnPosManager _spawnPosManager;
    [SerializeField] Transform _controllerPanelTransform;
    void Awake() { //Using awake to make Durations manager work properly in editor (also used in Visualiser Manager)
        _bpm = _actionCreator.Level.bpm;
        SecondsPerBeat = 60f / _bpm;
    }
    void OnEnable() {
        // Clearing leftover data from previous playtest; not required in main game as each attempt is a unique run of a scene
        _actionList.Clear();
        LevelEnd = 0;

        // Filling _actionList and _levelEnd variables
        foreach(Action action in _actionCreator.Actions) {
            _actionList.Add(action.Clone());
            var thisActionEnd = action.delay + action.beat*action.times + action.lifeTime;
            if(thisActionEnd > LevelEnd) {
                LevelEnd = thisActionEnd;
            }
        }
        LevelEnd += 5;

        _audioSource.clip = _audioLineManager.audioSource.clip;

        _firstUpdate = true;
        _offset = _spawnPosManager.offset; //sets the offset of the spawn of the player to the position of the audioLine
        _audioSource.time = _offset * SecondsPerBeat;
        _audioSource.Play();
        ClearChildren();
    }

    void OnDisable() {
        _audioSource.Stop();
        ClearChildren();
    }

    public override void TryEndLevel() {
        if(SongPositionInBeats/LevelEnd >= 1) {
            _editorMenuManager.TogglePlayTest(false); // false value means that it should toggle playtest off.
        }
    }

    #region ChildrenClear
    void ClearChildren() {
        for (var i = 0; i < transform.childCount; i++) {
            Destroy(transform.GetChild(i).gameObject);
        }
        for(var i = 0; i < _controllerPanelTransform.childCount; i++) {
            Destroy(_controllerPanelTransform.GetChild(i).gameObject);
        }
    }
    #endregion
}