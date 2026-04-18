public class NewsTitleEqualsFilter : WhereFilter<ServerNewsMetadata> {
    public NewsTitleEqualsFilter(string title)
        : base(x => x.Title == title) {
    }
}
