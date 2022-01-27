using PlayFab;
using PlayFab.ClientModels;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayFabLogin : MonoBehaviour
{
    public const string AuthGuidKey = "authorization-guid";

    [SerializeField] private Text _errorLabel;


    private void OnFailure(PlayFabError error)
    {
        var errorMessage = error.GenerateErrorReport();
        Debug.LogError($"Something went wrong: {errorMessage}");
        _errorLabel.text = errorMessage;
    }

    private void Start()
    {
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
        {
            Debug.LogError("PlayFab settings isn't correct! TitleId is missed!");
        }

        var needCreation = PlayerPrefs.HasKey(AuthGuidKey);
        var id = PlayerPrefs.GetString(AuthGuidKey, Guid.NewGuid().ToString());

        PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest()
        {
            CustomId = id,
            CreateAccount = !needCreation
        }, success =>
        { 
            PlayerPrefs.SetString(AuthGuidKey, id);
            Debug.Log($"Playfab Custom ID: {id}");
            SceneManager.LoadScene(1);
        }, OnFailure);
    }
}
