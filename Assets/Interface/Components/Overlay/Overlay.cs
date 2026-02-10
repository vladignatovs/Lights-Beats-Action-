using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public static class Overlay {
    public static void ToggleOverlay(bool? enabled) {
        try {
            var volume = Camera.main.GetComponent<PostProcessVolume>();
            volume.enabled = enabled ?? !volume.enabled;
        } catch { }
    }

    public static void ToggleOverlay(Camera camera, bool? enabled) {
        try {
            var volume = camera.GetComponent<PostProcessVolume>();            
            volume.enabled = enabled ?? !volume.enabled;
        } catch { }
    }
}