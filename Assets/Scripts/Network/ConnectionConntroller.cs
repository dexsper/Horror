using System;
using System.Threading.Tasks;
using FishNet.Managing;
using FishNet.Transporting.FishyUnityTransport;
using FishNet.Transporting.Multipass;
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
    private Multipass _multipassTransport;

    private void Awake()
    {
        _transport = GetComponent<FishyUnityTransport>();
        _networkManager = GetComponent<NetworkManager>();
        _lobbyManager = GetComponent<LobbyManager>();

        _multipassTransport = _networkManager.TransportManager.Transport as Multipass;

        _lobbyManager.OnJoinedLobby += OnJoinedLobby;
        _lobbyManager.OnLeftLobby += OnLeftLobby;
        _lobbyManager.OnJoinedLobbyUpdate += OnJoinedLobbyUpdate;

    }

    private void OnLeftLobby(object sender, EventArgs e)
    {
        if (_lobbyManager.IsLobbyHost())
        {
            _networkManager.ServerManager.StopConnection(true);
            return;
        }

        _networkManager.ClientManager.StopConnection();
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

        AddAuth();

        Allocation hostAllocation = await RelayService.Instance.CreateAllocationAsync(args.lobby.MaxPlayers);
        string joinCode = await RelayService.Instance.GetJoinCodeAsync(hostAllocation.AllocationId);

        _lobbyManager.UpdateConnectionCode(joinCode);
        _transport.SetRelayServerData(new RelayServerData(hostAllocation, "dtls"));
        
        _multipassTransport.SetClientTransport(1);
        _multipassTransport.StartConnection(true, 1);

        while (!_networkManager.IsServer)
        {
            await Task.Delay(100);
        }

        Connect(joinCode);
    }

    private void AddAuth()
    {
        if (!_networkManager.TryGetComponent(out UnityServicesAuthenticator authenticator))
        {
            authenticator = _networkManager.gameObject.AddComponent<UnityServicesAuthenticator>();
            _networkManager.ServerManager.SetAuthenticator(authenticator);
        }
    }

    private async void Connect(string joinCode)
    {
        AddAuth();

        if(_networkManager.TryGetComponent(out FakePlayer fakePlayer))
        {
            Destroy(fakePlayer);
        }

        JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

        _transport.SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

        _multipassTransport.SetClientTransport(1);
        _multipassTransport.StartConnection(false, 1);

        while (!_networkManager.ClientManager.Started)
        {
            await Task.Delay(100);
        }
    }
}
