using System;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

[Table("change_logs")]
public class ServerChangeLog : BaseModel {
    [PrimaryKey("id", false)]
    public long Id { get; set; }

    [Column("news_id")]
    public long NewsId { get; set; }

    [Column("admin_id")]
    public Guid AdminId { get; set; }

    [Column("action")]
    public string Action { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}

public class ChangeLogMetadata {
    public long id { get; set; }
    public long newsId { get; set; }
    public Guid adminId { get; set; }
    public string adminName { get; set; }
    public string action { get; set; }
    public DateTime createdAt { get; set; }
}
