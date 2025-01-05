using System.Collections.Generic;

public class Level {
    public int id;    
    public string levelName;
    public List<Action> actions;
    public float bpm;
    public string audioPath;
    public Level() {}
    public Level( string levelName, List<Action> actions, float bpm, string audioPath) {
        this.levelName = levelName;
        this.actions = actions;
        this.bpm = bpm;
        this.audioPath = audioPath;
    }
}