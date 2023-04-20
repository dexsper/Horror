using System;
using System.Collections.Generic;
using System.Linq;
using FishNet.Authenticating;
using FishNet.Broadcast;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Transporting;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public struct AuthenticationBroadcast : IBroadcast
{
    public string UserID;
}

public struct ResponseBroadcast : IBroadcast
{
    public bool Passed;
}

public static class ConnectionIdentity
{
    public static Dictionary<int, Player> Players = new Dictionary<int, Player>();

    public static Player GetPlayer(this NetworkConnection connection)
    {
        if (Players.TryGetValue(connection.ClientId, out Player player))
            return player;

        return null;
    }
}

public class UnityServicesAuthenticator : Authenticator
{
    public override event Action<NetworkConnection, bool> OnAuthenticationResult;

    public override void InitializeOnce(NetworkManager networkManager)
    {
        base.InitializeOnce(networkManager);

        base.NetworkManager.ClientManager.OnClientConnectionState += ClientManager_OnClientConnectionState;
        base.NetworkManager.ServerManager.RegisterBroadcast<AuthenticationBroadcast>(OnAuthBroadcast, false);
        base.NetworkManager.ClientManager.RegisterBroadcast<ResponseBroadcast>(OnResponseBroadcast);
    }

    /// <summary>
    /// Called when a connection state changes for the local client.
    /// </summary>
    private void ClientManager_OnClientConnectionState(ClientConnectionStateArgs args)
    {
        if (args.ConnectionState != LocalConnectionState.Started)
            return;

        AuthenticationBroadcast pb = new AuthenticationBroadcast()
        {
            UserID = AuthenticationService.Instance.PlayerId
        };

        base.NetworkManager.ClientManager.Broadcast(pb);
    }


    /// <summary>
    /// Received on server when a client sends the password broadcast message.
    /// </summary>
    /// <param name="conn">Connection sending broadcast.</param>
    /// <param name="data"></param>
    private void OnAuthBroadcast(NetworkConnection conn, AuthenticationBroadcast data)
    {
        if (conn.Authenticated)
        {
            conn.Disconnect(true);
            return;
        }

        var players = LobbyManager.Instance.JoinedLobby.Players;
        var player = players.FirstOrDefault(p => p.Id == data.UserID);

        bool passed = LobbyManager.Instance.IsLobbyHost() && player != null;

        ConnectionIdentity.Players[conn.ClientId] = player;

        SendAuthenticationResponse(conn, passed);
        OnAuthenticationResult?.Invoke(conn, passed);

        Debug.Log($"Authenticated client: {conn.ClientId}");
    }

    /// <summary>
    /// Received on client after server sends an authentication response.
    /// </summary>
    /// <param name="rb"></param>
    private void OnResponseBroadcast(ResponseBroadcast rb)
    {
        string result = (rb.Passed) ? "Authentication complete." : "Authenitcation failed.";
        NetworkManager.Log(result);
        if(rb.Passed)
            LobbyManager.Instance.LobbyUI.ActivateStartButton();
    }

    /// <summary>
    /// Sends an authentication result to a connection.
    /// </summary>
    private void SendAuthenticationResponse(NetworkConnection conn, bool authenticated)
    {
        ResponseBroadcast rb = new ResponseBroadcast()
        {
            Passed = authenticated
        };

        base.NetworkManager.ServerManager.Broadcast(conn, rb, false);
    }
}

