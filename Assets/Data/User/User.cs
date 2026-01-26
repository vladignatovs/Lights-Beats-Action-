using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

[Table("user")]
public class User: BaseModel {
    [PrimaryKey("id", false)]
    public int Id { get; set; }
    [Column("username")]
    public string Username { get; set; }
}

public enum Rights {
    User,
    Admin
}