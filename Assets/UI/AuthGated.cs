using UnityEngine;

/// <summary>
/// Base class for UI elements that should only be visible when the user is specifically either authenticated or not authenticated.
/// </summary>
public abstract class AuthGated : MonoBehaviour {
    public abstract bool ShowOnAuth { get; }

    protected virtual void Start() {
        var user = SupabaseManager.Instance.Auth;
        user.OnAuthenticationRequired += ToggleOnUnauth;
        user.OnAuthenticated += ToggleOnAuth;
        gameObject.SetActive(user.IsAuthenticated == ShowOnAuth);
    }

    protected virtual void OnDestroy() {
        var user = SupabaseManager.Instance.Auth;
        user.OnAuthenticationRequired -= ToggleOnUnauth;
        user.OnAuthenticated -= ToggleOnAuth;
    }

    void ToggleOnAuth() => gameObject.SetActive(ShowOnAuth);
    
    void ToggleOnUnauth() => gameObject.SetActive(!ShowOnAuth);
    
}