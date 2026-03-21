using static Supabase.Postgrest.Constants;

public class UsernameContainsFilter : OperatorFilter<ServerUserMetadata> {
    public UsernameContainsFilter(string searchText)
        : base("username", Operator.ILike, $"%{searchText}%") {
    }
}
