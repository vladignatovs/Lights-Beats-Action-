using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

[Table("user")]
public class User: BaseModel {
    [PrimaryKey("id", false)]
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public Rights Rights { get; set; }
}

public enum Rights {
    User,
    Admin
}