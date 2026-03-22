using System;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

[Table("friend_request")]
public class FriendRequestRecord : BaseModel {
    [PrimaryKey("id", false)]
    public long Id { get; set; }

    [Column("sender_id")]
    public Guid SenderId { get; set; }

    [Column("receiver_id")]
    public Guid ReceiverId { get; set; }

    [Column("accepted")]
    public bool? Accepted { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}

[Table("friend_request")]
public class FriendRequestInsert : BaseModel {
    [Column("receiver_id")]
    public Guid ReceiverId { get; set; }
}

[Table("friend_request")]
public class FriendRequestUpdate : BaseModel {
    [PrimaryKey("id", false)]
    public long Id { get; set; }

    [Column("accepted")]
    public bool? Accepted { get; set; }
}
