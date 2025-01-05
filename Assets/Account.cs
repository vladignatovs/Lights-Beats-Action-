public static class Account {
    public static string Username;
    public static bool LoggedIn { get { return Username != null;}}
    public static void LogIn(string username)=> Username = username;
    public static void LogOut() => Username = null;
}