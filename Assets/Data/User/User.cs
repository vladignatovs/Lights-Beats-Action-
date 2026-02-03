using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

[Table("user")]
public class User: BaseModel {
    [Column("username")]
    public string Username { get; set; }
}

public enum Rights {
    User,
    Admin
}