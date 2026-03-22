using System;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

[Table("block")]
public class BlockRecord : BaseModel {
    [Column("blocked_id")]
    public Guid Blocked { get; set; }
}

[Table("block")]
public class BlockRelationRecord : BaseModel {
    [Column("blocker_id")]
    public Guid Blocker { get; set; }

    [Column("blocked_id")]
    public Guid Blocked { get; set; }
}
