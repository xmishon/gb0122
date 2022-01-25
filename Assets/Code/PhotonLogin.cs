using Photon.Pun;
using Photon.Realtime;
using System;
using UnityEngine;

public class PhotonLogin : MonoBehaviourPunCallbacks
{
    public delegate void ConnectionStatusHandler(bool isConnected);
    public event ConnectionStatusHandler ConnectedToMaster;

    private string gameVersion = "1";

    #region publicMethods

    public void Connect()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
        }
    }

    public void Disconnect()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Successfully connected to Photon Master Server.");
        ConnectedToMaster?.Invoke(true);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        ConnectedToMaster?.Invoke(false);
    }

    #endregion


    #region privateMethods

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    #endregion
}
