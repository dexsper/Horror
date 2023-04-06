using System;
using System.Threading.Tasks;
using FishNet.Managing;
using FishNet.Transporting.FishyUnityTransport;
using Unity.Networking.Transport.Relay;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class ConnectionConntroller : MonoBehaviour
{
    private LobbyManager _lobbyManager;
    private FishyUnityTransport _transport;
    private NetworkManager _networkManager;

    private void Awake()
    {
        _transport = GetComponent<FishyUnityTransport>();
        _networkManager = GetComponent<NetworkManager>();
        _lobbyManager = GetComponent<LobbyManager>();

        _lobbyManager.OnJoinedLobby += OnJoinedLobby;
        _lobbyManager.OnJoinedLobbyUpdate += OnJoinedLobbyUpdate;
    }

    private void OnJoinedLobbyUpdate(object sender, LobbyEventArgs args)
    {
        if (_lobbyManager.IsLobbyHost())
            return;

        if (_networkManager.ClientManager.Started)
            return;

        Lobby lobby = args.lobby;
        
        if (lobby.Data.ContainsKey(LobbyManager.KEY_CONNECTION_CODE))
        {
            Connect(lobby.Data[LobbyManager.KEY_CONNECTION_CODE].Value);
        }
    }

    private async void OnJoinedLobby(object sender, LobbyEventArgs args)
    {
        if (!_lobbyManager.IsLobbyHost())
            return;

        Allocation hostAllocation = await RelayService.Instance.CreateAllocationAsync(args.lobby.MaxPlayers);
        string joinCode = await RelayService.Instance.GetJoinCodeAsync(hostAllocation.AllocationId);

        _lobbyManager.UpdateConnectionCode(joinCode);

        _transport.SetRelayServerData(new RelayServerData(hostAllocation, "dtls"));
        _networkManager.ServerManager.StartConnection();

        while (!_networkManager.IsServer)
        {
            await Task.Delay(100);
        }

        Connect(joinCode);
    }

    private async void Connect(string joinCode)
    {
        JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
        _transport.SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
        _networkManager.ClientManager.StartConnection();

        while (!_networkManager.ClientManager.Started)
        {
            await Task.Delay(100);
        }
    }
}
