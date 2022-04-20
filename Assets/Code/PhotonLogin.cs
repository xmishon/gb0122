using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class PhotonLogin : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject _playerList;

    private string _roomName = "CarsMultiplayer";

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        Connect();
    }

    public void Connect()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public void UpdateRoomName(string roomName)
    {
        _roomName = roomName;
    }

    public void OnCreateRoomButtonClicked()
    {
        if (!PhotonNetwork.InRoom)
        {
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 2;
            roomOptions.IsOpen = true;
            roomOptions.IsVisible = true;
            roomOptions.PublishUserId = true;
            PhotonNetwork.JoinOrCreateRoom(_roomName, roomOptions, TypedLobby.Default);
        }
        //PhotonNetwork.CreateRoom(_roomName);
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("Photon connected successfully");
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        RefreshPlayerList();
        Debug.Log("Successfully joined room!");
    }

    private void RefreshPlayerList()
    {
        int childCount = _playerList.transform.childCount;
        for(int i = 0; i < childCount; i++)
        {
            Destroy(_playerList.transform.GetChild(i).gameObject);
        }
        GameObject playerItem = Resources.Load<GameObject>("UI/PlayerItem");
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            GameObject instance = Instantiate(playerItem);
            instance.transform.SetParent(_playerList.transform);
            instance.GetComponent<PlayerElement>().SetItem(p);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        RefreshPlayerList();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
        RefreshPlayerList();
        Debug.Log("Room list updated.");
    }

    public void OnStartGameButtonClicked()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;

        PhotonNetwork.LoadLevel("TestTrack");
    }
}
