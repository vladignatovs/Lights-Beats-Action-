using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AuthenticationManager : MonoBehaviour {
    [SerializeField] Button _signInButton;
    [SerializeField] Button _signUpButton;
    [SerializeField] RectTransform _emailGroup;
    [SerializeField] GameObject _usernameGroup;
    [SerializeField] RectTransform _passwordGroup;
    [SerializeField] TMP_Text _errorText;
    [SerializeField] Button _confirmButton;

    bool _signInState = true;

    TMP_InputField _emailInput;
    TMP_InputField _usernameInput;
    TMP_InputField _passwordInput;
    RectTransform _errorRectTransform;
    RectTransform _confirmRectTransform;
    TMP_Text _signInLabel;
    TMP_Text _signUpLabel;

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

    void Start() {
        SupabaseManager.Instance.User.OnAuthenticationRequired += () => {
            gameObject.SetActive(true);
        };
        SupabaseManager.Instance.User.OnAuthenticated += () => {
            gameObject.SetActive(false);
        };
    } 

    void OnDestroy() {
        SupabaseManager.Instance.User.OnAuthenticationRequired -= () => {
            gameObject.SetActive(true);
        };
        SupabaseManager.Instance.User.OnAuthenticated -= () => {
            gameObject.SetActive(false);
        };
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
                // TODO: safify for prod
                var session = await SupabaseManager.Instance.User.SignIn(email, password);
                Debug.Log("Signed in: " + session.User.Email);
            } else {
                var session = await SupabaseManager.Instance.User.SignUp(email, username, password);
                // TODO: if session is valid, create a user profile with the username
                Debug.Log("Signed up: " + session.User.Email);
            }
            CleanUp();
            gameObject.SetActive(false);
        } catch (Exception e) {
            try {
                _errorText.text = "Sign in failed: " + JsonUtility.FromJson<AuthError>(e.Message).msg;
            } catch {
                _errorText.text = "Sign in failed due to an unknown error.";
            }
            Debug.LogWarning("Sign in failed: " + e.Message);
        }
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