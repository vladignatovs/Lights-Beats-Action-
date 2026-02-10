using UnityEngine;
public class BeatManager : BaseBeatManager {
    [Header("                LOADED LEVEL")]
    [SerializeField] LevelCompleteManager _levelCompleteManager;
    Level _level;
    void Awake() {
        // finds the level object with the same id
        _level = StateNameManager.Level;
        // sets the bpm and actionList from the found level
        _bpm = _level.bpm;
        foreach(Action action in _level.actions) {
            _actionList.Add(action.Clone());
            // setting the LevelEnd value, which is actions ending value
            var thisActionEnd = action.Delay + action.Beat*action.Times + action.LifeTime;
            if(thisActionEnd > LevelEnd) {
                LevelEnd = thisActionEnd;
            }
        }
        // adding 5 to levelend to make the ending feel a bit more natural
        LevelEnd += 5;
        // finds the seconds per beat, used in Awake() method to 
        // further make this value avaliable for DurationManager
        SecondsPerBeat = 60f / _bpm;
        // sets the audio clip to the one loaded by the levelLoader
        _audioSource.clip = StateNameManager.LoadedAudioClip;
    }

    void Start() {
        // used to make sure that the firstUpdate bool is true on the next retry
        _firstUpdate = true;
        _audioSource.time = _offset * SecondsPerBeat;
        _audioSource.Play();
    }
    
    public override void TryEndLevel() {
        if(SongPositionInBeats/LevelEnd >= 1) {
            _levelCompleteManager.LevelComplete(StateNameManager.Level.id);
        }
    }
}