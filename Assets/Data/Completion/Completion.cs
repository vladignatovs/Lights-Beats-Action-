
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;

[Table("completion")]
public class ServerCompletion : BaseModel {
    [PrimaryKey("level_id", false)]
    [Column("level_id")]
    public Guid LevelId { get; set; }

    [Column("percentage")]
    public float Percentage { get; set; }
    [Column("attempts")]
    public int Attempts { get; set; }
    [Column("accuracy")]
    public float Accuracy { get; set; }
}


public class Completion {
    public float percentage;
    public int attempts;
    public float accuracy;
}
