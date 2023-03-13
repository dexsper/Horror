using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher Instance;

    private void Awake()
    {
        Instance = this;
    }

    [SerializeField] private Transform roomListContainer;
    [SerializeField] private RoomListItem roomItemPrefab;
    
    private void Start()
    {
        EventManager.OnLeaveRoom += LeaveRoom;
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

    public void LeaveRoom()
    { 
        PhotonNetwork.LeaveRoom();
    }
    
    public override void OnJoinedRoom()
    {
        Debug.Log("Player joined the room");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Player failed to join the room");
    }

    public override void OnLeftRoom()
    {
        Debug.Log("Player Leave the room");
    }

    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
    }
    
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        for (int i = 0; i < roomListContainer.childCount; i++)
        {
            Destroy(roomListContainer.GetChild(i).gameObject);
        }
        
        for (int i = 0; i < roomList.Count; i++)
        {
            Instantiate(roomItemPrefab,roomListContainer).SetUp(roomList[i]);
        }
    }
}
