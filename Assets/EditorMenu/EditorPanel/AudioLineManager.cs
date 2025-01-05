using UnityEngine;

public class AudioLineManager : MonoBehaviour {
    [Header("Variables and GameObjects needed to work")]
    public AudioSource audioSource;
    public RectTransform rectTransform;
    public float bpm;
    [Header("Parameters used to work")]
    float secondsPerBeat;
    public float songPositionInSeconds;
    public float songPositionInBeats;
    float dspTimeAtStart;
    float offset; //in seconds
    // OnEnable works when the gameObject is Enabled/Active
    void OnEnable() {
        dspTimeAtStart = (float) AudioSettings.dspTime;
        secondsPerBeat = 60f / bpm; // Recieves the bpm from actionCreator
        rectTransform.position = Vector3.zero;
        float positionOffset = rectTransform.localPosition.x / 50 * secondsPerBeat; //sets the offset of the position of the line to the(100(localpositionx)/50 = 2(beats)*secondsPerBeat~1second)
        offset = Mathf.Max(0, positionOffset);
        audioSource.time = offset; //sets audio time to that value
        audioSource.Play();
    }
    void OnDisable() {
        audioSource.Stop();
    }

    // Update is called once per frame
    void Update() {
        songPositionInSeconds = (float) AudioSettings.dspTime - dspTimeAtStart + offset;
        songPositionInBeats = songPositionInSeconds / secondsPerBeat;
        rectTransform.localPosition = new Vector3(songPositionInBeats*50, rectTransform.localPosition.y, rectTransform.localPosition.z);
    }
}
