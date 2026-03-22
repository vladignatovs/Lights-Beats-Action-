using System;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

[Table("friendship")]
public class FriendshipRecord : BaseModel {
    [PrimaryKey("friend_id", false)]
    [Column("friend_id")]
    public Guid FriendId { get; set; }

    [PrimaryKey("friended_id", false)]
    [Column("friended_id")]
    public Guid FriendedId { get; set; }
}
