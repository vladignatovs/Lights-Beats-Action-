using System;
using System.Threading.Tasks;
using Supabase;
using UnityEngine;

public class AuthManager {

    public event System.Action OnAuthenticationRequired;
    public event System.Action OnAuthenticated;
    private Client _client;

    public AuthManager(Client client) {
        _client = client;
    }

    public async Task<Supabase.Gotrue.Session> TryAuthenticate() {
        try {
            var session = await _client.Auth.SetSession(
                PlayerPrefs.GetString("access_token", null),
                PlayerPrefs.GetString("refresh_token", null), 
                true
            );
            SaveSession(session);
            OnAuthenticated?.Invoke();
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
        var session = await _client.Auth.SignIn(email, password);
        SaveSession(session);
        return session;
    } 

    // TODO: store username in a different table
    public async Task<Supabase.Gotrue.Session> SignUp(string email, string username, string password) {
        Debug.Log("Trying to sign up");
        var session = await _client.Auth.SignUp(email, password);
        if (session == null) {
            Debug.Log("Sign up failed!");
            return null;
        }
        SaveSession(session);
        Debug.Log("Signed up!");
        return session;
    } 

    public async void SignOut() {
        await _client.Auth.SignOut();
        PlayerPrefs.DeleteKey("access_token");
        PlayerPrefs.DeleteKey("refresh_token");
        Debug.Log("Signed out!");
    }


    // TODO: playerprefs is dev only, use secure storage for prod
    void SaveSession(Supabase.Gotrue.Session session) {
        PlayerPrefs.SetString("access_token", session.AccessToken);
        PlayerPrefs.SetString("refresh_token", session.RefreshToken);
    }
}