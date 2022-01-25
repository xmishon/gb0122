using UnityEngine;

public class Reference
{
    private GameObject _loginPage;
    private GameObject _newPlayerPage;
    private GameObject _mainMenu;
    private GameObject _photonLogin;
    
    public GameObject LoginPage
    {
        get
        {
            if(_loginPage == null)
            {
                GameObject gameObject = Resources.Load<GameObject>("LoginPage");
                _loginPage = Object.Instantiate(gameObject);
            }
            return _loginPage;
        }
    }

    public GameObject NewPlayerPage
    {
        get
        {
            if (_newPlayerPage == null)
            {
                GameObject gameObject = Resources.Load<GameObject>("NewPlayerPage");
                _newPlayerPage = Object.Instantiate(gameObject);
            }
            return _newPlayerPage;
        }
    }

    public GameObject MainMenu
    {
        get
        {
            if (_mainMenu == null)
            {
                GameObject gameObject = Resources.Load<GameObject>("MainMenu");
                _mainMenu = Object.Instantiate(gameObject);
            }
            return _mainMenu;
        }
    }

    public GameObject PhotonLogin
    {
        get
        {
            if (_photonLogin == null)
            {
                GameObject gameObject = Resources.Load<GameObject>("PhotonLogin");
                _photonLogin = Object.Instantiate(gameObject);
            }
            return _photonLogin;
        }
    }
}
