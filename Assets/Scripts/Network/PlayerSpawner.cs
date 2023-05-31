using System.Collections.Generic;
using FishNet;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
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
        GetSpawn(out Vector3 position, out Quaternion rotation);

        var player = conn.GetPlayer();

        if (player == null) return;

        NetworkObject nob = _networkManager.GetPooledInstantiated(_prefab, _prefab.SpawnableCollectionId, true);

        nob.transform.SetPositionAndRotation(position, rotation);
        _networkManager.ServerManager.Spawn(nob, conn);

        UpdatePlayerModel(nob, player.Data[LobbyManager.KEY_PLAYER_CHARACTER].Value);
        Ads.Instance.ShowAd();
        AnalyticsEventManager.OnEvent("Player Spawned","Spawn","1");
    }

    [Server]
    public void RespawnPlayer(NetworkObject playerObject)
    {
        GetSpawn(out Vector3 position, out Quaternion rotation);
        playerObject.transform.SetPositionAndRotation(position, rotation);
    }

    [ObserversRpc(BufferLast = true)]
    private void UpdatePlayerModel(NetworkObject nob, string prefabName)
    {
        var prefab = _characterData[prefabName];
        nob.GetComponent<PlayerBehavior>().UpdateModel(prefab);
    }

    private void GetSpawn(out Vector3 pos, out Quaternion rot)
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
