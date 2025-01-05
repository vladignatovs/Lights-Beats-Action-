using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[System.Serializable] public class Action {
    public float beat; 
    public int times = 1;
    public float delay = 0; 
    public string gObject;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale = new Vector3(1, 1, 1);
    public float animationDuration;
    public float lifeTime;
    public List<int> groups = new() {0};
    //
    internal float firstBeat; // required to count times
    internal int timesDone; // required to count times
    public Action() {}
    public Action(float beat) {
        this.beat = beat;
        times = 1;
        delay = 0;
        scale = new Vector3(1, 1, 1);
        firstBeat = beat; // required to count times
    }

    public Action(float beat, int times, float delay, string gObject, Vector3 position, Quaternion rotation, Vector3 scale, float animationDuration, float lifeTime, List<int> groups) {
        this.beat = beat;
        this.times = times;
        this.delay = delay;
        this.gObject = gObject;
        this.position = position;
        this.rotation = rotation;
        this.scale = scale;
        this.animationDuration = animationDuration; 
        this.lifeTime = lifeTime;
        this.groups = new(groups);
        this.firstBeat = beat; // required to count times
    }

    public Action Clone() => new Action(beat, times, delay, gObject, position, rotation, scale, animationDuration, lifeTime, groups);
}