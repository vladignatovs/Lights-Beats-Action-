using static Supabase.Postgrest.Constants;

/// <summary>
/// Filter that matches levels owned by a specific user
/// </summary>
public class OwnedByFilter : OperatorFilter<ServerLevelMetadata> {
    public OwnedByFilter(string creatorId) 
        : base("creator_id", Operator.Equals, creatorId) {
    }
}
