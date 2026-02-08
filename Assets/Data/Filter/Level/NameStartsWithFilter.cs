using static Supabase.Postgrest.Constants;

public class NameStartsWithFilter : OperatorFilter<ServerLevelMetadata> {
    public NameStartsWithFilter(string prefix) 
        : base("name", Operator.ILike, $"{prefix}%") {
    }
}
