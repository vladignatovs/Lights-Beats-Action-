using System;
using System.Threading.Tasks;
using Supabase;
using UnityEngine;

public sealed class AuthManager : DataManager {
    public AuthManager(Client client) : base(client) { }
    public event System.Action OnAuthenticationRequired;
    public event System.Action OnAuthenticated;
    public bool IsAuthenticated => _client.Auth.CurrentUser != null;

    public async Task<Supabase.Gotrue.Session> TryAuthenticate() {
        try {
            var session = await SetSession();
            Debug.Log("Authenticated via saved session!");
            return session;
        } catch(Exception e) {
            // TODO: try to authenticate via steam
            Debug.LogError($"Authentication failed: {e.Message}");
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

    public async void SignOut() {
        await _client.Auth.SignOut();
        PlayerPrefs.DeleteKey("access_token");
        PlayerPrefs.DeleteKey("refresh_token");
        OnAuthenticationRequired?.Invoke();
        Debug.Log("Signed out!");
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

    // TODO: playerprefs is dev only, use secure storage for prod
    void SaveSession(Supabase.Gotrue.Session session) {
        PlayerPrefs.SetString("access_token", session.AccessToken);
        PlayerPrefs.SetString("refresh_token", session.RefreshToken);
    }
}