using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class AccountMenu : MonoBehaviour
{
    [SerializeField] private PlayFabAccountManager _playFabAccountManager;
    [SerializeField] private GameObject _optionsWindow;
    [SerializeField] private GameObject _accountWindow;
    [SerializeField] private GameObject _signInWindow;
    [SerializeField] private GameObject _createWindow;

    public void ShowAccountOptions()
    {
        if(_playFabAccountManager.AccountInfoResult == null)
        {
            _playFabAccountManager.GetAccountInfo(OnGetAccountInfoSuccess);
        }
        if (string.IsNullOrEmpty(_playFabAccountManager.AccountInfoResult.AccountInfo.PrivateInfo.Email))
        {
            _optionsWindow.SetActive(true);
            _accountWindow.SetActive(false);
        }
        else
        {
            _accountWindow.SetActive(true);
            _optionsWindow.SetActive(false);
        }
        _signInWindow.SetActive(false);
        _createWindow.SetActive(false);
    }

    public void OnGetAccountInfoSuccess(GetAccountInfoResult result)
    {
        _playFabAccountManager.AccountInfoResult = result;
        if (string.IsNullOrEmpty(result.AccountInfo.PrivateInfo.Email))
        {
            _optionsWindow.SetActive(true);
            _accountWindow.SetActive(false);
        }
        else
        {
            _accountWindow.SetActive(true);
            _optionsWindow.SetActive(false);
        }
        _signInWindow.SetActive(false);
        _createWindow.SetActive(false);
    }
}
