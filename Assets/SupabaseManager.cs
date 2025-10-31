using UnityEngine;
using Supabase;

public interface ISupabaseClientProvider {
    Client Client { get; set; }
    bool IsInitialized { get; set; }
}

public class SupabaseManager : MonoBehaviour, ISupabaseClientProvider {
    public static SupabaseManager Instance { get; private set; }
    public Client Client { get; set; }
    public bool IsInitialized { get; set; }

    [Header("Config (use ScriptableObject or CI in production)")]
    [SerializeField] string _url = "https://tvctalpryctdeqmwvujg.supabase.co";
    [SerializeField] string _anonKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InR2Y3RhbHByeWN0ZGVxbXd2dWpnIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NjE5MTUxMTIsImV4cCI6MjA3NzQ5MTExMn0.Gg2QvLbfeegOM13e0mvGnVi2QHiYCL4bPJYVgGMXau4";
    [SerializeField] bool _autoConnectRealtime = false;

    async void Awake() {
        var options = new SupabaseOptions {
            AutoConnectRealtime = _autoConnectRealtime
        };

        Client = new Client(
            _url,
            _anonKey,
            options
            );
        await Client.InitializeAsync();

        Debug.Log("Connected to Supabase!");
    }
}