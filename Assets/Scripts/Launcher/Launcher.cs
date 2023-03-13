using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Random = UnityEngine.Random;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher Instance;

    private void Awake()
    {
        Instance = this;
    }

    [SerializeField] private Transform roomListContainer;
    [SerializeField] private RoomListItem roomItemPrefab;

    [SerializeField] private Transform playerList;
    [SerializeField] private PlayerListItem playerNamePrefab;
        
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
        PhotonNetwork.NickName = $"Player  {Random.Range(0, 20)}";
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

        Player[] players = PhotonNetwork.PlayerList;
        for (int i = 0; i < players.Length; i++)
        {
            Instantiate(playerNamePrefab,playerList).SetUp(players[i]);
        }
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

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(playerNamePrefab,playerList).SetUp(newPlayer);
    }
}
