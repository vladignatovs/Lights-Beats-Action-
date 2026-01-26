using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AuthenticationManager : AuthGated {
    public override bool ShowOnAuth => false;
    [SerializeField] Button _signInButton;
    [SerializeField] Button _signUpButton;
    [SerializeField] RectTransform _emailGroup;
    [SerializeField] GameObject _usernameGroup;
    [SerializeField] RectTransform _passwordGroup;
    [SerializeField] TMP_Text _errorText;
    [SerializeField] Button _confirmButton;
    [SerializeField] Overlay _overlay;

    bool _signInState = true;

    TMP_InputField _emailInput;
    TMP_InputField _usernameInput;
    TMP_InputField _passwordInput;
    RectTransform _errorRectTransform;
    RectTransform _confirmRectTransform;
    TMP_Text _signInLabel;
    TMP_Text _signUpLabel;

    protected override void Start() {
        base.Start();
        var user = SupabaseManager.Instance.Auth;
        user.OnAuthenticationRequired += () => _overlay.ToggleOverlay(true);
        user.OnAuthenticated += () => _overlay.ToggleOverlay(false);
        _overlay.ToggleOverlay(!user.IsAuthenticated);
    }

    protected override void OnDestroy() {
        base.OnDestroy();
        var user = SupabaseManager.Instance.Auth;
        user.OnAuthenticationRequired -= () => _overlay.ToggleOverlay(true);
        user.OnAuthenticated -= () => _overlay.ToggleOverlay(false);
    }

    void Awake() {
        // or get childcomponent
        _emailInput = _emailGroup.GetComponentInChildren<TMP_InputField>();
        _usernameInput = _usernameGroup.GetComponentInChildren<TMP_InputField>();
        _passwordInput = _passwordGroup.GetComponentInChildren<TMP_InputField>();

        _errorRectTransform = _errorText.GetComponent<RectTransform>();
        _confirmRectTransform = _confirmButton.GetComponent<RectTransform>();

        _signInLabel  = _signInButton.GetComponentInChildren<TMP_Text>();
        _signUpLabel  = _signUpButton.GetComponentInChildren<TMP_Text>();

        _emailInput.onValueChanged.AddListener(_ => ValidateConfirm());
        _passwordInput.onValueChanged.AddListener(_ => ValidateConfirm());
        _usernameInput.onValueChanged.AddListener(_ => ValidateConfirm());

        ValidateConfirm();
    }

    public void TogglePanel() {
        _signInState = !_signInState;
        _signInButton.interactable = !_signInState;
        _signUpButton.interactable = _signInState;
        _signInLabel.fontStyle = _signInState ? FontStyles.Underline : FontStyles.Normal;
        _signUpLabel.fontStyle = _signInState ? FontStyles.Normal : FontStyles.Underline;

        _usernameGroup.SetActive(!_signInState);

        float deltaY = _signInState ? 90f : -90f;
        _passwordGroup.anchoredPosition += new Vector2(0, deltaY);
        _errorRectTransform.anchoredPosition += new Vector2(0, deltaY);
        _confirmRectTransform.anchoredPosition += new Vector2(0, deltaY);

        CleanUp();
        ValidateConfirm();
    }

    public async void Confirm() {
        var email = _emailInput.text;
        var username = _usernameInput.text;
        var password = _passwordInput.text;

        try {
            if(_signInState) {
                var session = await SupabaseManager.Instance.Auth.SignIn(email, password);
                Debug.Log("Signed in: " + session.User.Email);
            } else {
                // TODO: its possible for sign up to silently fail and create user to run, or vice versa, must be handled
                var session = await SupabaseManager.Instance.Auth.SignUp(email, password);
                var user = await SupabaseManager.Instance.User.CreateUser(username);
                // TODO: if session is valid, create a user profile with the username
                Debug.Log("Signed up: " + session.User.Email);
            }
            CleanUp();
        } catch (Exception e) {
            try {
                _errorText.text = "Sign in failed: " + JsonUtility.FromJson<AuthError>(e.Message).msg;
            } catch {
                _errorText.text = "Sign in failed due to an unknown error.";
            }
            Debug.LogWarning("Sign in failed: " + e.Message);
        }
    }

    // TODO: keep guest session alive, give guest an option to authenticate later
    public void Guest() {
        _overlay.ToggleOverlay(false);
        CleanUp();
        gameObject.SetActive(false);
    }

    void CleanUp() {
        _emailInput.text = "";
        _usernameInput.text = "";
        _passwordInput.text = "";
        _errorText.text = "";
    }

    void ValidateConfirm() {
        if (_signInState) {
            _confirmButton.interactable =
                !string.IsNullOrWhiteSpace(_emailInput.text) &&
                !string.IsNullOrWhiteSpace(_passwordInput.text);
        } else {
            _confirmButton.interactable =
                !string.IsNullOrWhiteSpace(_emailInput.text) &&
                !string.IsNullOrWhiteSpace(_usernameInput.text) &&
                !string.IsNullOrWhiteSpace(_passwordInput.text);
        }
    }

    [Serializable]
    class AuthError {
        public string msg;
    }
}