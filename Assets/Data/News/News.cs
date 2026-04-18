using System;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

public static class NewsCategories {
    public const string Announcement = "announcement";
    public const string Update = "update";
    public const string Other = "other";
}

[Table("news")]
public class ServerNews : BaseModel {
    [PrimaryKey("id", false)]
    public long Id { get; set; }

    [Column("title")]
    public string Title { get; set; }

    [Column("content")]
    public string Content { get; set; }

    [Column("category")]
    public string Category { get; set; }

    [Column("thumbnail")]
    public string ThumbnailUrl { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}

[Table("news")]
public class ServerNewsMetadata : BaseModel {
    [PrimaryKey("id", false)]
    public long Id { get; set; }

    [Column("title")]
    public string Title { get; set; }

    [Column("category")]
    public string Category { get; set; }

    [Column("thumbnail")]
    public string ThumbnailUrl { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}

[Table("news")]
public class NewsInsert : BaseModel {
    [Column("title")]
    public string Title { get; set; }

    [Column("content")]
    public string Content { get; set; }

    [Column("category")]
    public string Category { get; set; }

    [Column("thumbnail")]
    public string ThumbnailUrl { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}

public class News {
    public long id { get; set; }
    public string title { get; set; }
    public string content { get; set; }
    public string category { get; set; }
    public string thumbnailUrl { get; set; }
    public DateTime createdAt { get; set; }
}

public class NewsMetadata {
    public long id { get; set; }
    public string title { get; set; }
    public string category { get; set; }
    public string thumbnailUrl { get; set; }
    public DateTime createdAt { get; set; }
}
