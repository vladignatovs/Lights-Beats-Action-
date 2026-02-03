using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

[Table("level")]
public class ServerLevel : BaseModel {
    [PrimaryKey("id", false)]
    public Guid Id { get; set; }
    [Column("name")]
    public string Name { get; set;}
    [Column("audio_path")]
    public string AudioPath { get; set; }
    [Column("bpm")]
    public float Bpm { get; set; }
    [Column("actions")]
    public List<ServerAction> Actions { get; set; }
}

[Table("level")]
public class ServerLevelMetadata : BaseModel {
    [PrimaryKey("id", false)]
    public Guid Id { get; set; }
    [Column("creator_id")]
    public Guid CreatorId { get; set; }
    [Column("creator_username")]
    public string CreatorUsername { get; set; }
    [Column("name")]
    public string Name { get; set; }
    [Column("audio_path")]
    public string AudioPath { get; set; }
    [Column("bpm")]
    public float Bpm { get; set; }
}

[System.Serializable]
public class ServerAction
{
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

    // IMPORTANT: JArray prevents the Postgrest int-array converter from kicking in.
    public JArray Groups { get; set; } = new JArray(0);
}