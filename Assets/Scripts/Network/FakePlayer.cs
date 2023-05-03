using System;
using System.Collections.Generic;
using FishNet;
using FishNet.Connection;
using FishNet.Managing.Server;
using FishNet.Transporting;
using Unity.Services.Lobbies.Models;
using UnityEngine;

#if UNITY_EDITOR || DEVELOPMENT_BUILD

public class FakePlayer : MonoBehaviour
{
    [SerializeField] private string _characterName;

    private ServerManager _serverManager;

    private void Awake()
    {
        _serverManager = InstanceFinder.ServerManager;
        _serverManager.OnRemoteConnectionState += OnRemoteConnectionState;
    }

    private void OnRemoteConnectionState(NetworkConnection conn, RemoteConnectionStateArgs args)
    {
        if (args.ConnectionState == RemoteConnectionState.Started)
        {
            ConnectionIdentity.Players[conn.ClientId] = new Player($"Fake_{_serverManager.Clients.Count}")
            {
                Data = new Dictionary<string, PlayerDataObject>
                {
                    {LobbyManager.KEY_PLAYER_NAME, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, $"Fake_{_serverManager.Clients.Count}_Player")},
                    {LobbyManager.KEY_PLAYER_CHARACTER, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, _characterName)}
                }
            };
        }
    }
}

#endif