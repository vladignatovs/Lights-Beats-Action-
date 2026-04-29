using System;
using System.Threading.Tasks;
using Supabase;
using UnityEngine;

public sealed class AuthManager : DataManager {
    const string GuestModePlayerPrefsKey = "guest_mode";

    public AuthManager(Client client) : base(client) { }
    public event System.Action OnAuthenticationRequired;
    public event System.Action OnAuthenticated;
    public event Action<bool> OnGuestModeChanged;
    public bool IsAuthenticated => _client.Auth.CurrentUser != null;
    public string Email => _client.Auth.CurrentUser?.Email ?? string.Empty;
    public bool IsGuestMode { get; private set; } = PlayerPrefs.GetInt(GuestModePlayerPrefsKey, 0) == 1;

    public async Task<Supabase.Gotrue.Session> TryAuthenticate() {
        try {
            var session = await SetSession();
            Debug.Log("Authenticated via saved session!");
            return session;
        } catch(Exception e) {
            Debug.LogError($"Authentication failed: {e.Message}");

            if (IsGuestMode) {
                OnGuestModeChanged?.Invoke(true);
                return null;
            }

            OnAuthenticationRequired?.Invoke();
            return null;
        }
    }

    public async Task<Supabase.Gotrue.Session> SignIn(string email, string password) {
        if(string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password)) {
            throw new ArgumentException("{ msg: \"Email and password must not be empty\" }");
        }
        var session = await _client.Auth.SignIn(email, password);
        session = await SetSession(session);
        return session;
    } 

    public async Task<Supabase.Gotrue.Session> SignUp(string email, string password) {
        if(string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password)) {
            throw new ArgumentException("{ msg: \"Email and password must not be empty\" }");
        }
        var session = await _client.Auth.SignUp(email, password);
        if (session == null) {
            Debug.Log("Sign up failed!");
            return null;
        }
        session = await SetSession(session);
        Debug.Log("Signed up!");
        return session;
    } 

    public async Task SignOut() {
        DisableGuestMode();
        await _client.Auth.SignOut();
        PlayerPrefs.DeleteKey("access_token");
        PlayerPrefs.DeleteKey("refresh_token");
        OnAuthenticationRequired?.Invoke();
        Debug.Log("Signed out!");
    }

    public async Task UpdateEmail(string email) {
        if (string.IsNullOrWhiteSpace(email)) {
            throw new ArgumentException("{ msg: \"Email must not be empty\" }");
        }

        await _client.Auth.Update(new Supabase.Gotrue.UserAttributes {
            Email = email.Trim()
        });

        SaveCurrentSession();
    }

    public async Task UpdatePassword(string password) {
        if (string.IsNullOrWhiteSpace(password)) {
            throw new ArgumentException("{ msg: \"Password must not be empty\" }");
        }

        await _client.Auth.Update(new Supabase.Gotrue.UserAttributes {
            Password = password
        });

        SaveCurrentSession();
    }

    public void EnableGuestMode() {
        if (IsGuestMode) {
            return;
        }

        IsGuestMode = true;
        PlayerPrefs.SetInt(GuestModePlayerPrefsKey, 1);
        OnGuestModeChanged?.Invoke(true);
    }

    public void DisableGuestMode() {
        if (!IsGuestMode) {
            return;
        }

        IsGuestMode = false;
        PlayerPrefs.DeleteKey(GuestModePlayerPrefsKey);
        OnGuestModeChanged?.Invoke(false);
    }


    async Task<Supabase.Gotrue.Session> SetSession(Supabase.Gotrue.Session session) {
        var sess = await _client.Auth.SetSession(session.AccessToken, session.RefreshToken);

        SaveSession(sess);
        OnAuthenticated?.Invoke();
        return sess;
    }

    async Task<Supabase.Gotrue.Session> SetSession() {
        var sess = await _client.Auth.SetSession(
            PlayerPrefs.GetString("access_token", null), 
            PlayerPrefs.GetString("refresh_token", null),
            true);
        SaveSession(sess);
        OnAuthenticated?.Invoke();
        return sess;
    }

    void SaveSession(Supabase.Gotrue.Session session) {
        PlayerPrefs.SetString("access_token", session.AccessToken);
        PlayerPrefs.SetString("refresh_token", session.RefreshToken);
    }

    void SaveCurrentSession() {
        var session = _client.Auth.CurrentSession;
        if (session != null) {
            SaveSession(session);
        }
    }
}
