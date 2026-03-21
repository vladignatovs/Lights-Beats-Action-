public class UsernameEqualsFilter : WhereFilter<ServerUserMetadata> {
    public UsernameEqualsFilter(string username)
        : base(x => x.Username == username) {
    }
}
