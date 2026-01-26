

using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class Overlay : MonoBehaviour {
    public void ToggleOverlay(bool enabled) {
        Camera.main.GetComponent<PostProcessVolume>().enabled = enabled;
    }
}