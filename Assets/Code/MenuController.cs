using UnityEngine;

public class MenuController : MonoBehaviour
{

    private NewPlayerView _newPlayerView;
    private LoginView _loginView;
    private PlayFabLogin _playFabLogin;
    private Reference _reference;
    private GameObject _loginPage;
    private GameObject _newPlayerPage;
    private GameObject _mainMenuPage;
    private MainMenuView _mainMenuView;
    private GameObject _photonLoginObject;
    private PhotonLogin _photonLogin;

    private void Start()
    {
        _reference = new Reference();

        InitializeLoginPage();
        InitializeNewUserPage();
        _newPlayerPage.SetActive(false);

        _playFabLogin = new PlayFabLogin();
        _playFabLogin.SuccessfulLogin += OnSuccessfulLogin;

        InitializeMainMenu();
        _mainMenuPage.SetActive(false);

    }

    private void OnSubmitLogin(string login)
    {
        _playFabLogin.Authorize(login);
    }

    private void OnCreateNewUser(string login)
    {
        _playFabLogin.CreateAccount(login);
    }

    private void OnNewPlayerButtonPress()
    {
        _loginPage.SetActive(false);
        _newPlayerPage.SetActive(true);
    }

    private void OnReturnButtonPress()
    {
        _loginPage.SetActive(true);
        _newPlayerPage.SetActive(false);
    }

    private void OnSuccessfulLogin()
    {
        _loginPage.SetActive(false);
        _newPlayerPage.SetActive(false);
        _mainMenuPage.SetActive(true);
        _photonLogin.Connect();
    }

    private void InitializeLoginPage()
    {
        _loginPage = _reference.LoginPage;
        _loginView = _loginPage.GetComponent<LoginView>();
        _loginView.SubmitLogin += OnSubmitLogin;
        _loginView.NewPlayerButtonPress += OnNewPlayerButtonPress;
    }

    private void InitializeNewUserPage()
    {
        _newPlayerPage = _reference.NewPlayerPage;
        _newPlayerView = _newPlayerPage.GetComponent<NewPlayerView>();
        _newPlayerView.SubmitLogin += OnCreateNewUser;
        _newPlayerView.ReturnButtonPress += OnReturnButtonPress;
    }

    private void InitializeMainMenu()
    {
        _mainMenuPage = _reference.MainMenu;
        _mainMenuView = _mainMenuPage.GetComponent<MainMenuView>();
        _photonLoginObject = _reference.PhotonLogin;
        _photonLogin = _photonLoginObject.GetComponent<PhotonLogin>();
        _photonLogin.ConnectedToMaster += _mainMenuView.UpdateConnectionStatus;
        _photonLogin.ConnectedToMaster += this.UpdateConnectionStatus;
        _mainMenuView.ConnectButtonPress += _photonLogin.Connect;
    }

    private void UpdateConnectionStatus(bool isConnected)
    {
        if (isConnected)
        {
            _mainMenuView.ConnectButtonPress -= _photonLogin.Connect;
            _mainMenuView.ConnectButtonPress += _photonLogin.Disconnect;
        }
        else
        {
            _mainMenuView.ConnectButtonPress -= _photonLogin.Disconnect;
            _mainMenuView.ConnectButtonPress += _photonLogin.Connect;
        }
    }
}
