using PlayFab;
using PlayFab.ClientModels;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayFabAccountManager : MonoBehaviour
{
    [SerializeField] private Text _titleLabel;
    [SerializeField] private Text _createErrorLabel;
    [SerializeField] private Text _signInErrorLabel;
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _accountMenu;

    private string _username;
    private string _email;
    private string _password;

    public GetAccountInfoResult AccountInfoResult;

    private void Start()
    {
        _titleLabel.text = "Loading";
        GetAccountInfo(OnGetAccountSuccess);
    }

    public void GetAccountInfo(Action<GetAccountInfoResult> onSuccessCallback)
    {
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), onSuccessCallback, OnFailure);
    }

    private void OnGetAccountSuccess(GetAccountInfoResult result)
    {
        AccountInfoResult = result;
        _titleLabel.text = $"Welcome, Player ID {result.AccountInfo.PlayFabId}\r\n" +
            $"Created: {result.AccountInfo.Created.ToString()}\r\n" +
            $"Email: {result.AccountInfo.PrivateInfo.Email?.ToString()}";
        string customId = result.AccountInfo.CustomIdInfo.CustomId;
        if (!string.IsNullOrEmpty(customId))
        {
            PlayerPrefs.SetString(PlayFabLogin.AuthGuidKey, result.AccountInfo.CustomIdInfo.CustomId);
        }
    }

    public void UpdateUsername(string username)
    {
        _username = username;
    }

    public void UpdateEmail(string email)
    {
        _email = email;
    }

    public void UpdatePassword(string password)
    {
        _password = password;
    }

    private Action<RegisterPlayFabUserResult> resultCallback;

    public void CreateAccount()
    {
        resultCallback = OnCreateSuccess;

        PlayFabClientAPI.RegisterPlayFabUser(new RegisterPlayFabUserRequest
        {
            Username = _username,
            Email = _email,
            Password = _password,
            RequireBothUsernameAndEmail = true
        },
        resultCallback,
        OnFailure);
    }

    public void SignIn()
    {
        PlayFabClientAPI.LoginWithPlayFab(new LoginWithPlayFabRequest
        {
            Username = _username,
            Password = _password
        }, OnSignInSuccess, OnFailure);
    }

    public void SignOut()
    {
        PlayFabClientAPI.ForgetAllCredentials();
        PlayerPrefs.DeleteKey(PlayFabLogin.AuthGuidKey);
        SceneManager.LoadScene(0);
    }

    private void OnCreateSuccess(RegisterPlayFabUserResult result)
    {
        Debug.Log($"Creation success: {_username}");
        PlayFabClientAPI.LinkCustomID(new LinkCustomIDRequest
        {
            CustomId = PlayerPrefs.GetString(PlayFabLogin.AuthGuidKey),
            ForceLink = true
        }, reslut =>
        {
            Debug.Log($"Successfully created new account with CustomID: {PlayerPrefs.GetString(PlayFabLogin.AuthGuidKey)}");
            GetAccountInfo(OnGetAccountSuccess);
        }, error =>
        {
            var errorMessage = error.GenerateErrorReport();
            Debug.LogError($"Something went wrong: {errorMessage}");
        }
        );

        ShowMainMenu();
    }

    private void OnSignInSuccess(LoginResult result)
    {
        Debug.Log($"Sign In success: {_username}");
        GetAccountInfo(OnGetAccountSuccess);
        ShowMainMenu();
    }

    private void OnFailure(PlayFabError error)
    {
        var errorMessage = error.GenerateErrorReport();
        Debug.LogError($"Something went wrong: {errorMessage}");
        _createErrorLabel.text = errorMessage;
        _signInErrorLabel.text = errorMessage;
    }

    private void ShowMainMenu()
    {
        _mainMenu.SetActive(true);
        _accountMenu.SetActive(false);
    }

    public void Back()
    {
        _createErrorLabel.text = "";
        _signInErrorLabel.text = "";
    }
}
