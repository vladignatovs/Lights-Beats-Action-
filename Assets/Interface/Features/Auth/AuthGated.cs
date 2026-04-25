using UnityEngine;

/// <summary>
/// Base class for UI elements that should only be visible when the user is specifically either authenticated or not authenticated.
/// </summary>
public abstract class AuthGated : MonoBehaviour {
    public abstract bool ShowOnAuth { get; }
    protected virtual bool IgnoreGuestMode => false;

    protected virtual void Start() {
        var user = SupabaseManager.Instance.Auth;
        user.OnAuthenticationRequired += RefreshState;
        user.OnAuthenticated += RefreshState;
        user.OnGuestModeChanged += HandleGuestModeChanged;
        RefreshState();
    }

    protected virtual void OnDestroy() {
        var user = SupabaseManager.Instance.Auth;
        user.OnAuthenticationRequired -= RefreshState;
        user.OnAuthenticated -= RefreshState;
        user.OnGuestModeChanged -= HandleGuestModeChanged;
    }

    void HandleGuestModeChanged(bool _) {
        RefreshState();
    }

    void RefreshState() {
        var auth = SupabaseManager.Instance.Auth;
        ApplyState(IsAllowed(auth));
    }

    protected virtual bool IsAllowed(AuthManager auth) {
        if (auth.IsAuthenticated) {
            return ShowOnAuth;
        }

        if (!IgnoreGuestMode && auth.IsGuestMode) {
            return false;
        }

        return !ShowOnAuth;
    }

    protected virtual void ApplyState(bool isAllowed) {
        gameObject.SetActive(isAllowed);
    }
}
