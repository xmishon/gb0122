using PlayFab;
using PlayFab.ClientModels;
using System;
using UnityEngine;

public class PlayFabLogin
{
    public event Action SuccessfulLogin;

    public void Authorize(string login)
    {
        LogIn(login, false);
    }

    public void CreateAccount(string login)
    {
        LogIn(login, true);
    }

    private void LogIn(string login, bool createAccount)
    {
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
        {
            Debug.LogError("PlayFabSettings isn't correct");
            Notification notification = new Notification();
            notification.ShowError("PlayFabSettings isn't correct");
            return;
        }
        LoginWithCustomIDRequest request = new LoginWithCustomIDRequest { CustomId = login, CreateAccount = createAccount };
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Succesful PlayFab API call!");
        Notification notification = new Notification();
        notification.Show("Successful login on PlayFab");
        SuccessfulLogin?.Invoke();
    }

    private void OnLoginFailure(PlayFabError error)
    {
        var errorMessage = error.GenerateErrorReport();
        Debug.LogError($"PlayFab API call failure: {errorMessage}");
        Notification notification = new Notification();
        notification.ShowError(errorMessage);
    }
}
