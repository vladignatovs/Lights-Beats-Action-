using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AudioDropdownManager : MonoBehaviour {
    [SerializeField] TMP_Dropdown dropdown;
    AudioClip[] audioClips;
    bool _isPopulated;

    void Awake() {
        EnsurePopulated();
    }

    public void EnsurePopulated() {
        if(_isPopulated) return;
        var options = new List<string>();
        audioClips = Resources.LoadAll<AudioClip>("Audio/");
        dropdown.ClearOptions();
        options.Add("None");
        foreach(var clip in audioClips) {
            options.Add(clip.name);
        }
        dropdown.AddOptions(options);
        _isPopulated = true;
    }
}
