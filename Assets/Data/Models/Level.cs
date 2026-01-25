using System.Collections.Generic;

public class Level {
    public int localId;
    public int serverId;  
    public string name;
    public List<Action> actions;
    public float bpm;
    public string audioPath;
    public Level() { }
    public Level(int localId, string name, List<Action> actions, float bpm, string audioPath)
    {
        this.localId = localId;
        this.name = name;
        this.actions = actions;
        this.bpm = bpm;
        this.audioPath = audioPath;
    }
}

public class LevelFile {
    public Meta Meta { get; set; }
    public List<Action> Actions { get; set; }
}

public class Meta {
    public int LocalId { get; set; }
    public int ServerId { get; set; }
    public string name { get; set; }
    public float bpm { get; set; }
    public string audioPath { get; set; }
}

/*
public interface ILevelRepo
{
    Task Publish(Level level);
    Task Unpublish(Level level);
}

public class LevelRepo : ILevelRepo
{
    readonly Client client;

    public LevelRepo(Client client)
    {
        this.client = client;
    }

    public Task Publish(Level level) => client.From<Level>().Insert(level);

    public Task Unpublish(Level level) => client.From<Level>().Delete(level);
}
*/