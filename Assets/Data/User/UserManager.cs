using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Supabase;
using Supabase.Postgrest.Interfaces;
using static Supabase.Postgrest.Constants;

public class UserManager : DataManager {
    public UserManager(Client client) : base(client) {
        SupabaseManager.Instance.Auth.OnAuthenticated += async () => {
            await LoadUserAsync();
        };
    }

    public event Action<string> OnNameChanged;

    public string Name { get; private set; } = "Guest";

    void SetName(string value)
    {
        Name = string.IsNullOrWhiteSpace(value) ? "Guest" : value;
        OnNameChanged?.Invoke(Name);
    }

    public async Task<User> CreateUser(string username)
    {
        var user = new User {
            Username = username
        };
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

    public async Task<UserMetadata> LoadUserById(Guid userId) {
        var user = await _client
            .From<User>()
            .Where(x => x.Id == userId)
            .Select(u => new object[] { u.Id, u.Username })
            .Single();

        if (user == null) {
            return null;
        }

        return new UserMetadata {
            id = user.Id,
            username = user.Username,
            isBlocked = false,
        };
    }

    public async Task<(List<UserMetadata> items, int totalCount)> LazyLoadUsers(
        int offset,
        int limit,
        List<IDataFilter<ServerUserMetadata>> filters = null
    ) {
        var table = _client.From<ServerUserMetadata>();
        var query = table.Select("id,username");

        if (filters != null) {
            foreach (var filter in filters) {
                query = filter.Apply(query);
            }
        }

        IPostgrestTable<ServerUserMetadata> countQuery = _client.From<ServerUserMetadata>();
        if (filters != null) {
            foreach (var filter in filters) {
                countQuery = filter.Apply(countQuery);
            }
        }

        var getTask = query.Range(offset, offset + limit - 1).Get();
        var countTask = countQuery.Count(CountType.Exact);

        await Task.WhenAll(getTask, countTask);

        var response = await getTask;
        int totalCount = await countTask;

        var items = (response.Models ?? new())
            .Select(ToLocalUserMetadata)
            .ToList();

        return (items, totalCount);
    }

    public async Task DeleteCurrentUser() {
        if (_client.Auth.CurrentUser == null) {
            throw new InvalidOperationException("Cannot delete user without an authenticated session.");
        }

        if (!Guid.TryParse(_client.Auth.CurrentUser.Id, out var currentUserId)) {
            throw new InvalidOperationException("Authenticated user id is not a valid GUID.");
        }

        await _client
            .From<User>()
            .Where(x => x.Id == currentUserId)
            .Delete();

        SetName("Guest");
    }

    static UserMetadata ToLocalUserMetadata(ServerUserMetadata user) {
        return new UserMetadata {
            id = user.Id,
            username = user.Username,
        };
    }
}
