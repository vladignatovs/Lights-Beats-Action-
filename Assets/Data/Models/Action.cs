using System.Collections.Generic;

// TODO: find a proper place for action and level locally used classes
[System.Serializable] public class Action {
    public float Beat { get; set; }
    public int Times { get; set; } = 1;
    public float Delay { get; set; } = 0; 
    public string GObject { get; set; }
    public float PositionX { get; set; } = 0;
    public float PositionY { get; set; } = 0;
    public float Rotation { get; set; }
    public float ScaleX { get; set; } = 1;
    public float ScaleY { get; set; } = 1;
    public float AnimationDuration { get; set; }
    public float LifeTime { get; set; }
    public List<int> Groups { get; set; } = new() {0};
    //
    internal float FirstBeat { get; set; } // required to count times
    internal int TimesDone { get; set; } // required to count times
    public Action() {}
    public Action(float beat) {
        Beat = beat;
        Times = 1;
        Delay = 0;
        FirstBeat = beat; // required to count times
    }

    public Action(float beat, int times, float delay, string gObject, float positionX, float positionY, float rotation,  float scaleX, float scaleY, float animationDuration, float lifeTime, List<int> groups) {
        Beat = beat;
        Times = times;
        Delay = delay;
        GObject = gObject;
        PositionX = positionX;
        PositionY = positionY;
        Rotation = rotation;
        ScaleX = scaleX;
        ScaleY = scaleY;
        AnimationDuration = animationDuration; 
        LifeTime = lifeTime;
        Groups = new(groups);
        FirstBeat = beat; // required to count times
    }

    public Action Clone() => new(Beat, Times, Delay, GObject, PositionX, PositionY, Rotation, ScaleX, ScaleY, AnimationDuration, LifeTime, Groups);
}