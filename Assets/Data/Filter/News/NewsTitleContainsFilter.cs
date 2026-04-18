using static Supabase.Postgrest.Constants;

public class NewsTitleContainsFilter : OperatorFilter<ServerNewsMetadata> {
    public NewsTitleContainsFilter(string title)
        : base("title", Operator.ILike, $"%{title}%") {
    }
}
