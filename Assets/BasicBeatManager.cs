using System.Collections.Generic;
using UnityEngine;

public abstract class BasicBeatManager : MonoBehaviour {
    [Header ("                BEAT INFO")]
    [SerializeField] float _secondsPerBeat;
    public float SecondsPerBeat {
        get { return _secondsPerBeat; }
        protected set { _secondsPerBeat = value; }
    }
    [SerializeField] float _dspTimeAtStart;
    float _dspTimeAtPause;
    float _dspTimeAtResume;
    [Header ("                BEAT PLAYING")]
    [SerializeField] float _songPositionInSeconds;

    [SerializeField] float _songPositionInBeats;
    public float SongPositionInBeats {
        get { return _songPositionInBeats; }
        set { _songPositionInBeats = value; }
    }
    [SerializeField] protected float _offset;
    [Header ("                BEAT SETTINGS")]
    [SerializeField] protected AudioSource _audioSource;
    protected float _bpm;
    [SerializeField] protected List<Action> _actionList;
    protected bool _firstUpdate = true;
    [Header("                LOADED LEVEL")]
    private float _levelEnd;
    public float LevelEnd {
        get { return _levelEnd; }
        protected set { _levelEnd = value;}
    }

    #region Update
    void Update() {
        FirstUpdateTimeFix();

        SyncAudioAndTimeWithPause();

        if(!LogicManager.isPaused) {
            // this gets the position in the song without considering the last attempts
            _songPositionInSeconds = (float) AudioSettings.dspTime - _dspTimeAtStart - (_dspTimeAtResume - _dspTimeAtPause) + (_offset * _secondsPerBeat);
            // gets the position in beats of the song
            SongPositionInBeats = _songPositionInSeconds / _secondsPerBeat;

            foreach(Action action in _actionList) {
                if((action.beat + action.delay) < SongPositionInBeats && action.times > 0) {

                    var (newAction, shouldContinue) = CalculateActionCall(action);
                    action.beat = newAction.beat;
                    action.times = newAction.times;
                    action.timesDone = newAction.timesDone;

                    if (shouldContinue) continue;

                    SpawnGameObject(action);
                }
            }
        }

        TryEndLevel();
    }
    #endregion
    #region FirstUpdate
    /// <summary>
    /// This method is used to set the _dspTimeAtStart property to the correct one, as first Update() method is delayed from Start() method
    /// making the dspTimeAtStart property inefficient if set then.
    /// </summary>
    void FirstUpdateTimeFix() {
        if(_firstUpdate) {
            _dspTimeAtStart = (float) AudioSettings.dspTime;
            _dspTimeAtResume = _dspTimeAtPause = 0;
            _firstUpdate = false;
        }
    }
    #endregion
    #region SyncWithPause
    /// <summary>
    /// This method is used to make sure that the audio and _dspTimeAtPause/Resume are all synced with the paused state of the game.
    /// </summary>
    void SyncAudioAndTimeWithPause() {
        if(LogicManager.isPaused && _audioSource.isPlaying) {
            _audioSource.Pause();
            _dspTimeAtPause += (float) AudioSettings.dspTime;
        } else if(!LogicManager.isPaused && !_audioSource.isPlaying) {
            _audioSource.Play();
            _dspTimeAtResume += (float) AudioSettings.dspTime;
        }
    }
    #endregion
    #region ActionCall
    /// <summary>
    /// This method is mainly used to change the values of the parsed action, as well as return true, if some conditions are met to 
    /// further break the foreach() loop in the Update() method.
    /// </summary>
    /// <remarks> NOTE: should only be called whenever the action is allowed to take place (i.e. if songPos is more than the beat+delay and
    /// times > 0). </remarks>
    /// <param name="a">Action object, which will provide values for the check and suffer changes in the process.</param>
    /// <returns> Tuple, which is a way to return multiple values with a single method. </returns>
    (Action newAction, bool shouldBreak) CalculateActionCall(Action a) {
        
        // imitates action calling, lowering the times value by 1, adding 1 to timesDone and adding firstBeat to beat value.
        void ActionCall() {
            a.times--; 
            a.timesDone++;
            a.beat += a.firstBeat;
        }

        bool shouldContinue = false;
        // sets the value of the actualOffset, which is further used to avoid redundancy.
        float actualOffset = _offset - a.delay; // i.e. (x + y < z) == (x < z - y)

        // checks if beat is less than actualOffset (i.e. 20 < 50-10, meaning that the action should've happened before the start)
        if(a.beat < actualOffset) {
            // method used to check whether action should be active by comparing with actualOffset, spawning gameObject with according
            // timer value
            void CheckLifeTime() {
                if((a.beat < actualOffset) && ((a.beat + (a.gObject.Contains("Telegraph") ? a.animationDuration*1.25 : a.lifeTime)) > actualOffset)) {
                    SpawnGameObject(a, actualOffset - a.beat);
                    shouldContinue = true;
                }
            }

            // checks if firstBeat is not zero (can't be negative as beat can't be negative), as it won't provide any change to the beat value.
            if(a.firstBeat != 0) {
                // sets the value of amountOfTimesBeforeCall, which is basically a calculated amount of times needed before an action
                // is able to exceed actualOffset value (i.e. (10-2)/2 = 8/2 = 4 <- times before action is called). 
                int amountOfTimesBeforeCall = (int)Mathf.Ceil((actualOffset - a.beat) / a.firstBeat);
                // sets the value of amountOfTimesPossible, which is the maximum amount of times an action can repeat before making a.times value go to 0.
                int amountOfTimesPossible = a.times;
                // sets the value of actualTimes, which is minimal value between amountOfTimesBeforeCall and amountOfTimesPossible.
                // (i.e Min(10,5) = 5. Can repeat 5 times)
                // (i.e. Min(10,15) = 10. Can repeat 10 times before beat exceeds actualOffset)
                int actualTimes = Mathf.Min(amountOfTimesBeforeCall, amountOfTimesPossible);
                // loops through actualTimes simulating action calling and checking if action would still be active by then
                for (int i = 0; i < actualTimes; i++) {
                    CheckLifeTime();
                    ActionCall();
                }
                // checks if there are no times left and beat didn't exceed the actualOffset, meaning that it wouldn't be called as after 
                // calling the beat value will always exceed songPos with firstBeat not being zero, so will signal to continue and not spawn gameObject
                if(a.times == 0 && a.beat <= actualOffset) {
                    shouldContinue = true;
                }
            // else the firstBeat IS zero, so it checks for lifetime of each time, then sets times to zero, 
            // and singals to continue, as theres nothing to be changed with the repetition.
            } else { 
                for(int i = 0; i < a.times; i++) {
                    CheckLifeTime();
                }
                a.times = 0;
                shouldContinue = true;
            }
        // else the beat value is NOT less than actualOffset, meaning that it wasn't supposed to happen before the start, 
        // and skipping the unwanted calculations.
        } else {
            ActionCall();
        }
        // returns a tuple with the updated action and a boolean value to signal whether to break the loop or not.
        return (a, shouldContinue);
    }
    #endregion
    #region ObjectSpawn
    void SpawnGameObject(Action a) {
        var gameObj = Resources.Load<GameObject>("Prefabs/" + a.gObject);

        GameObject aInstance = Instantiate(gameObj, a.position, a.rotation, transform);
        aInstance.transform.localScale = a.scale;
        
        var durationsManager = aInstance.GetComponent<DurationsManager>();
        durationsManager.AnimationDuration = a.animationDuration;
        durationsManager.LifeTime = a.lifeTime;

        aInstance.GetComponent<GroupManager>().groups.AddRange(a.groups);

        if(a.gObject.Contains("Controller")) {
            aInstance.GetComponent<ControllerGroupManager>().targetGroup = (int)a.scale.x;
            var controllerManager = aInstance.GetComponent<ControllerManager>();
            controllerManager.SetAllUniqueValues(a);
        }
    }
    void SpawnGameObject(Action a, float time) {
        var gameObj = Resources.Load<GameObject>("Prefabs/" + a.gObject);

        GameObject aInstance = Instantiate(gameObj, a.position, a.rotation, transform);
        aInstance.transform.localScale = a.scale;
        
        var durationsManager = aInstance.GetComponent<DurationsManager>();
        durationsManager.AnimationDuration = a.animationDuration;
        durationsManager.LifeTime = a.lifeTime;
        durationsManager.Timer = time*SecondsPerBeat;

        aInstance.GetComponent<GroupManager>().groups.AddRange(a.groups);

        if(a.gObject.Contains("Controller")) {
            aInstance.GetComponent<ControllerGroupManager>().targetGroup = (int)a.scale.x;
            var controllerManager = aInstance.GetComponent<ControllerManager>();
            controllerManager.SetAllUniqueValues(a);
        }
    }
    #endregion
    #region LevelEnd
    public abstract void TryEndLevel();
    #endregion
}