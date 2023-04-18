using System.Collections.Generic;
using System.Linq;
using FishNet;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private CharactersData _characterData;
    [SerializeField] private NetworkObject _prefab;
    [SerializeField] private List<Transform> _spawns;

    private NetworkManager _networkManager;
    private LobbyManager _lobbyManager;

    private int _nextSpawn = 0;

    private void Awake()
    {
        _networkManager = InstanceFinder.NetworkManager;
        _lobbyManager = LobbyManager.Instance;
    }

    [Server]
    public void SpawnPlayer(NetworkConnection conn)
    {
        Vector3 position;
        Quaternion rotation;
        SetSpawn(out position, out rotation);

        var player = conn.GetPlayer();

        if (player == null) return;

        var prefab = _characterData[player.Data[LobbyManager.KEY_PLAYER_CHARACTER].Value];

        Debug.Log(prefab);

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
