using static Supabase.Postgrest.Constants;

public class NewsTitleStartsWithFilter : OperatorFilter<ServerNewsMetadata> {
    public NewsTitleStartsWithFilter(string title)
        : base("title", Operator.ILike, $"{title}%") {
    }
}
