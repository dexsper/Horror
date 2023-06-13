using System;
using System.Collections.Generic;
using FishNet;
using FishNet.Connection;
using FishNet.Managing.Server;
using FishNet.Transporting;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class FakePlayer : MonoBehaviour
{
    public string PlayerName;
    public string CharacterName;

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
            if (string.IsNullOrEmpty(PlayerName))
            {
                PlayerName = $"Player";
            }

            ConnectionIdentity.Players[conn.ClientId] = new Player($"Fake_{_serverManager.Clients.Count}")
            {
                Data = new Dictionary<string, PlayerDataObject>
                {
                    {LobbyManager.KEY_PLAYER_NAME, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, PlayerName)},
                    {LobbyManager.KEY_PLAYER_CHARACTER, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, CharacterName)}
                }
            };
        }
    }
}
