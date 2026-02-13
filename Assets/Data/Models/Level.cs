using System;
using System.Collections.Generic;

public class LevelMetadata {
    public int id { get; set; }
    public Guid? serverId { get; set; }
    public Guid? creatorId { get; set; }
    public string? creatorUsername { get; set; }
    public string name { get; set; }
    public float bpm { get; set; }
    public string audioPath { get; set; }
    public float startOffset { get; set; }
}

public class Level {
    public int id;
    public Guid? serverId;
    public string name;
    public List<Action> actions;
    public float bpm;
    public string audioPath;
    public float startOffset;
    public Level() { }
    public Level(int id, string name, List<Action> actions, float bpm, string audioPath, float startOffset = 0f)
    {
        this.id = id;
        this.name = name;
        this.actions = actions;
        this.bpm = bpm;
        this.audioPath = audioPath;
        this.startOffset = startOffset;
    }
}