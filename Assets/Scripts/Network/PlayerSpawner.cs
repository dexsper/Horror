using System.Collections.Generic;
using System.Linq;
using FishNet;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private NetworkObject _prefab;
    [SerializeField] private List<Transform> _spawns;

    private NetworkManager _networkManager;
    private int _nextSpawn = 0;

    private void Awake()
    {
        _networkManager = InstanceFinder.NetworkManager;
    }

    [Server]
    public void SpawnPlayer(NetworkConnection conn)
    {
        Vector3 position;
        Quaternion rotation;
        SetSpawn(out position, out rotation);

        string connectionAddress = conn.GetAddress();
        Debug.Log("Connection id: " + conn.ClientId);
        Debug.Log("Connection address: " + connectionAddress);

        Debug.Log("Players:");

        foreach(var p in LobbyManager.Instance.JoinedLobby.Players)
        {
            Debug.Log(p.Id);
        }

        Player lobbyPlayer = LobbyManager.Instance.JoinedLobby.Players.First(p => p.Id == connectionAddress);

        Debug.Log(lobbyPlayer.Data[LobbyManager.KEY_PLAYER_CHARACTER]);

        NetworkObject nob = _networkManager.GetPooledInstantiated(_prefab, true);
        nob.transform.SetPositionAndRotation(position, rotation);
        _networkManager.ServerManager.Spawn(nob, conn);
    }

    private void SetSpawn(out Vector3 pos, out Quaternion rot)
    {
        if (_spawns.Count == 0)
        {
            pos = Vector3.zero;
            rot = Quaternion.identity;

            return;
        }

        Transform result = _spawns[_nextSpawn];

        pos = result.position;
        rot = result.rotation;

        _nextSpawn = (_nextSpawn + 1) % _spawns.Count;
    }
}
