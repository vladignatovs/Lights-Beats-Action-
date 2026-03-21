using static Supabase.Postgrest.Constants;

public class UsernameStartsWithFilter : OperatorFilter<ServerUserMetadata> {
    public UsernameStartsWithFilter(string prefix)
        : base("username", Operator.ILike, $"{prefix}%") {
    }
}
