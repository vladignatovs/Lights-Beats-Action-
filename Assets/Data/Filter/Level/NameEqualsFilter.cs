
public class NameEqualsFilter : WhereFilter<ServerLevelMetadata> {
    public NameEqualsFilter(string name) 
        : base(x => x.Name == name) {
    }
}
