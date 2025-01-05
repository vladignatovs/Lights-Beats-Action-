using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
public class AccountManager : MonoBehaviour {

    [SerializeField] GameObject _registrationPanel;
    [SerializeField] InputField _rUsernameInput;
    [SerializeField] InputField _rPasswordInput;
    [SerializeField] Button _registerButton;

    [SerializeField] GameObject _loginPanel;
    [SerializeField] InputField _lUsernameInput;
    [SerializeField] InputField _lPasswordInput;
    [SerializeField] Button _loginButton;

    public void CallRegister() {
        StartCoroutine(Register());
    }
    
    IEnumerator Register() {
        WWWForm form =  new();
        form.AddField("username", _rUsernameInput.text);
        form.AddField("password", _rPasswordInput.text);

        // Create a UnityWebRequest with the URL
        UnityWebRequest wReq = UnityWebRequest.Post("http://localhost/sqlconnect/register.php", form);
        // Send the request and wait for a response
        yield return wReq.SendWebRequest();

        // Check if there is any error in the request
        if (wReq.result == UnityWebRequest.Result.Success) {
            if (wReq.downloadHandler.text == "success") {
                Account.LogIn(_rUsernameInput.text);
            } else {
                Debug.Log("Failed to create user. Error #" + wReq.downloadHandler.text);
            }
        } else {
            Debug.Log("Request failed: " + wReq.error);
        }
    }

    public void CallLogin() {
        StartCoroutine(Login());
    }

    IEnumerator Login() {
        WWWForm form =  new();
        form.AddField("username", _lUsernameInput.text);
        form.AddField("password", _lPasswordInput.text); 

        // Create a UnityWebRequest with the URL
        UnityWebRequest wReq = UnityWebRequest.Post("http://localhost/sqlconnect/login.php", form);
        // Send the request and wait for a response
        yield return wReq.SendWebRequest();

        // Check if there is any error in the request
        if (wReq.result == UnityWebRequest.Result.Success) {
            if (wReq.downloadHandler.text == "success") {
                Account.LogIn(_lUsernameInput.text);
            } else {
                Debug.Log("Failed to login. Error #" + wReq.downloadHandler.text);
            }
        } else {
            Debug.Log("Request failed: " + wReq.error);
        }
    }

    public void ShowRegister() => _registrationPanel.SetActive(!_loginPanel.activeSelf);
    public void HideRegister() => _registrationPanel.SetActive(false);
    public void ShowLogin() => _loginPanel.SetActive(!_registrationPanel.activeSelf);
    public void HideLogin() => _loginPanel.SetActive(false);

    public void VerifyInputs() {
        if(_registrationPanel.activeSelf) {
            _registerButton.interactable = _rUsernameInput.text.Length >= 2 && _rPasswordInput.text.Length >= 8;
        } else if(_loginPanel.activeSelf)  {
            _loginButton.interactable = _lUsernameInput.text.Length >= 2 && _lPasswordInput.text.Length >= 8;
        }
    }
} 
