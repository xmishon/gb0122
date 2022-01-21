using System;
using UnityEngine;
using UnityEngine.UI;

public class NewPlayerView : MonoBehaviour
{
    public delegate void SubmitLoginHandler(string login);
    public event SubmitLoginHandler SubmitLogin;

    public event Action ReturnButtonPress;

    [SerializeField] Text _inputText;

    public void Submit()
    {
        SubmitLogin(_inputText.text);
    }

    public void Return()
    {
        ReturnButtonPress?.Invoke();
    }
}
