using System;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

[Table("message")]
public class Message : BaseModel {
    [PrimaryKey("id", false)]
    public long Id { get; set; }

    [Column("sender_id")]
    public Guid SenderId { get; set; }

    [Column("receiver_id")]
    public Guid ReceiverId { get; set; }

    [Column("message")]
    public string Content { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}

[Table("message")]
public class MessageInsert : BaseModel {
    [Column("sender_id")]
    public Guid SenderId { get; set; }

    [Column("receiver_id")]
    public Guid ReceiverId { get; set; }

    [Column("message")]
    public string Content { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}
