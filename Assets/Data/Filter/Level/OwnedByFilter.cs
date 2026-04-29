using static Supabase.Postgrest.Constants;

public class OwnedByFilter : OperatorFilter<ServerLevelMetadata> {
    public OwnedByFilter(string creatorId) 
        : base("creator_id", Operator.Equals, creatorId) {
    }
}
