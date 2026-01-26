using Supabase;

public abstract class DataManager {
    internal Client _client;

    public DataManager(Client client) {
        _client = client;
    }
}