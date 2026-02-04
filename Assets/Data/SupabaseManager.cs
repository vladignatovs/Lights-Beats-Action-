using UnityEngine;
using Supabase;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Collections.Generic;

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

    // Data services
    public AuthManager Auth { get; private set; }
    public UserManager User { get; private set; }
    public LevelManager Level { get; private set; }

    private async void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        var options = new SupabaseOptions {
            AutoConnectRealtime = _autoConnectRealtime
        };

        Client = new Client(_url, _anonKey, options);
        await Client.InitializeAsync();
        IsInitialized = true;

        // Initializing all the data services
        Auth = new(Client);
        User = new(Client);
        Level = new(Client);

        // Try to authenticate user
        await Auth.TryAuthenticate();
        Debug.Log("Connected to Supabase!");
    }
}