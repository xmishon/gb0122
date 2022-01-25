using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuView : MonoBehaviour
{
    private const string CONNECTED = "Connected";
    private const string DISCONNECTED = "Disonnected";
    private const string CONNECT = "Connect";
    private const string DISCONNECT = "Disconnect";

    public event Action PlayButtonPress;
    public event Action ConnectButtonPress;

    [SerializeField] private Button _playButton;
    [SerializeField] private Button _connectGameServerButton;
    [SerializeField] private Text _serverStatus;

    public void Play()
    {
        PlayButtonPress?.Invoke();
    }

    public void Connect()
    {
        ConnectButtonPress?.Invoke();
    }

    public void UpdateConnectionStatus(bool isConnected)
    {
        if (isConnected)
        {
            _serverStatus.text = CONNECTED;
            _serverStatus.color = Color.green;
            _connectGameServerButton.GetComponentInChildren<Text>().text = DISCONNECT;
        }
        else
        {
            _serverStatus.text = DISCONNECTED;
            _serverStatus.color = Color.red;
            _connectGameServerButton.GetComponentInChildren<Text>().text = CONNECT;
        }
    }
}
