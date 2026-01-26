using System.Threading.Tasks;
using Supabase;

public class UserManager : DataManager {
    public UserManager(Client client) : base(client) {
        SupabaseManager.Instance.Auth.OnAuthenticated += async () => {
            await LoadUserAsync();
        };
    }

    public event System.Action<string> OnNameChanged;

    public string Name { get; private set; } = "Guest";

    void SetName(string value)
    {
        Name = string.IsNullOrWhiteSpace(value) ? "Guest" : value;
        OnNameChanged?.Invoke(Name);
    }

    public async Task<User> CreateUser(string username)
    {
        var user = new User { Username = username };
        var usr = await _client.From<User>().Insert(user);

        SetName(usr.Model?.Username);
        return usr.Model;
    }

    public async Task LoadUserAsync()
    {
        var user = await _client
            .From<User>()
            .Select(u => new object[] { u.Username })
            .Single();

        SetName(user?.Username);
    }
}