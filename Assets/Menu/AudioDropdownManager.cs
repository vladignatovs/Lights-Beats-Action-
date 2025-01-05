using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioDropdownManager : MonoBehaviour {
    AudioClip[] audioClips;
    public Dropdown dropdown;

    void Start() {
        dropdown = GetComponent<Dropdown>();
        var options = new List<string>();
        audioClips = Resources.LoadAll<AudioClip>("Audio/");
        dropdown.ClearOptions();
        options.Add("None");
        foreach(var clip in audioClips) {
            options.Add(clip.name);
        }
        dropdown.AddOptions(options);
        //what
    }
}
