using System;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

[Table("user")]
public class User: BaseModel {
    [PrimaryKey("id", false)]
    public Guid Id { get; set; }

    [Column("username")]
    public string Username { get; set; }
}

[Table("user")]
public class ServerUserMetadata : BaseModel {
    [PrimaryKey("id", false)]
    public Guid Id { get; set; }

    [Column("username")]
    public string Username { get; set; }
}

public class UserMetadata {
    public Guid id { get; set; }
    public string username { get; set; }
}

public enum Rights {
    User,
    Admin
}
