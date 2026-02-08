using static Supabase.Postgrest.Constants;

public class NameContainsFilter : OperatorFilter<ServerLevelMetadata> {
    public NameContainsFilter(string searchText) 
        : base("name", Operator.ILike, $"%{searchText}%") {
    }
}
