using UnityEngine;

public class DurationsManager : MonoBehaviour {
    float _secondsPerBeat;
    public float AnimationDuration; //IN BEATS
    // controls the duration of:
    // - fading in (telegraphs)
    // - fading out (attacks) (pillar, explosion, etc)
    /* p.s. telegraphs don't need fading out duration (default 0.25f, perhaps should change to beats),
    as well as attacks don't need fading in duration (telegraphs pretty much serve that function) */
    public float LifeTime; //IN BEATS
    // controls the duration of:
    // - lifeTime (attacks) (explosion, pillar, missle, rain(?), etc)
    /* p.s. telegraphs don't need lifeTime, as they just slowly appear and quickly disappear(will make less sense otherwise),
    as well as certain attacks don't need lifeTime (explosion) */
    public float Timer; //IN SECONDS, SHOULD BE ZERO BY DEFAULT
    // added as an attempt to try and control the durations of gameobjects in editor manually.
    void Awake() {
        var logic = GameObject.FindGameObjectWithTag("Logic");
        if(logic.TryGetComponent<BeatManager>(out var beatManager)) {
            _secondsPerBeat = beatManager.SecondsPerBeat;
        } else {
            _secondsPerBeat = logic.GetComponent<EditorBeatManager>().SecondsPerBeat;
        }
    }

    void Update() {
        Timer += Time.deltaTime;
    }
    
    public float getAnimationDurationInSeconds() {
        return AnimationDuration*_secondsPerBeat;
    }

    public float getLifeTimeInSeconds() {
        return LifeTime*_secondsPerBeat;
    }

}
