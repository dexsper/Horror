using System;
using Photon.Pun;
using UnityEngine;

public class Launcher : MonoBehaviourPunCallbacks
{
    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        Debug.Log("Connecting to Master");
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined The Lobby");
    }

    public void CreateRoom(string roomName)
    {
        PhotonNetwork.CreateRoom(roomName);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Player joined the room");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Player failed to join the room");
    }
}
