using System;
using UnityEngine;
using UnityEngine.UI;

public class LoginView : MonoBehaviour
{
    public delegate void SubmitLoginHandler(string login);
    public event SubmitLoginHandler SubmitLogin;

    public event Action NewPlayerButtonPress;


    [SerializeField] Text _inputText;

    public void Submit()
    {
        SubmitLogin?.Invoke(_inputText.text);
    }

    public void CreateNewPlayer()
    {
        NewPlayerButtonPress?.Invoke();
    }
}
