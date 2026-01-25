using System;
using System.Collections.Generic;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

[Table("level")]
public class LevelPublished : BaseModel {
    [PrimaryKey("id", false)]
    public long Id { get; set; }

    public string Name { get; set; }

    [Column("audio_path")]
    public string AudioPath { get; set; }

    public float Bpm { get; set; }

    // Use JSONB for actions
    public List<Action> Actions { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}