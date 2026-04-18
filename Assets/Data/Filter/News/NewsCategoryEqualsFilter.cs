using static Supabase.Postgrest.Constants;

public class NewsCategoryEqualsFilter : OperatorFilter<ServerNewsMetadata> {
    public NewsCategoryEqualsFilter(string category)
        : base("category", Operator.Equals, category) {
    }
}
