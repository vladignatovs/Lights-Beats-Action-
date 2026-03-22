using System;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

[Table("block")]
public class BlockRecord : BaseModel {
    [Column("blocked_id")]
    public Guid Blocked { get; set; }
}
