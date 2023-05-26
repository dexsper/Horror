using System;
using FishNet;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Managing.Scened;
using FishNet.Object;
using FishNet.Utility;
using UnityEngine;

public class GameController : NetworkBehaviour
{
    [SerializeField] private int _winReward = 10;
    [SerializeField, Scene] private string _deathRoomScene;

    private static GameController _instance;

    private PlayerSpawner _spawner;
    private SceneManager _sceneManager;
    private NetworkManager _networkManager;

    public static event Action OnGameEnded;
    
    public static GameController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameController>();
            }

            return _instance;
        }

        private set => _instance = value;
    }

    private void Awake()
    {
        Instance = this;

        _spawner = GetComponent<PlayerSpawner>();
        _networkManager = InstanceFinder.NetworkManager;

        _sceneManager = InstanceFinder.SceneManager;
        _sceneManager.OnClientPresenceChangeEnd += OnClientPresenceChangeEnd;

        DeathRoom.OnPlayerLeave += OnPlayerLeaveDeathRoom;
        Generator.OnRepaired += OnGeneratorRepaired;

        PlayerBehavior.OnDead += OnPlayerDead;
        PlayerBehavior.OnLeaved += OnPlayerLeaveZone;
        PlayerBehavior.OnPlayerRemoved += OnPlayerRemoved;
    }

    private void Start()
    {
        if (base.IsServer)
        {
            _sceneManager.LoadGlobalScenes(new SceneLoadData(_deathRoomScene));
        }
    }

    #region Game State

    [Server]
    private void OnGeneratorRepaired(Generator generator)
    {
        for (int i = 0; i < Generator.Generators.Count; i++)
        {
            if (!Generator.Generators[i].IsRepaired)
                return;
        }

        Debug.Log("All generators repaired");

        for (int i = 0; i < LeaveZone.LeaveZones.Count; i++)
        {
            LeaveZone.LeaveZones[i].Activate();
        }
    }

    [Server]
    private void OnPlayerLeaveZone(PlayerBehavior player)
    {
        player.transform.position = Vector3.zero;

        for (int i = 0; i < PlayerBehavior.Players.Count; i++)
        {
            if (!PlayerBehavior.Players[i].IsLeaved)
                return;
        }

        for (int i = 0; i < PlayerBehavior.Players.Count; i++)
        {
            FinishGame_RPC(PlayerBehavior.Players[i].Owner, _winReward);
        }
    }

    [Server]
    private void OnPlayerLeaveDeathRoom(PlayerBehavior player)
    {
        _spawner.RespawnPlayer(player.NetworkObject);

        player.Health.Restore();
    }

    [Server]
    private void OnPlayerDead(PlayerBehavior player)
    {
        DeathRoom.Instance.AddPlayer(player);
    }

    [Server]
    private void OnPlayerRemoved()
    {
        if(PlayerBehavior.Players.Count == 0)
        {
            _networkManager.ServerManager.StopConnection(true);
        }
    }


    #endregion

    #region Connection State

    [Server]
    private void OnClientPresenceChangeEnd(ClientPresenceChangeEventArgs args)
    {
        if (args.Scene != gameObject.scene) return;

        OnClientJoin(args.Connection);
    }

    [Server]
    private void OnClientJoin(NetworkConnection connection)
    {
        if (PlayerBehavior.Players.Exists(p => p.Owner == connection))
            return;

        _spawner.SpawnPlayer(connection);
    }

    [TargetRpc]
    private void FinishGame_RPC(NetworkConnection conn, int reward)
    {
        PlayerEconomy.Instance.IncrementBalance(reward);
        
        LobbyManager.Instance.LeaveLobby();
        OnGameEnded?.Invoke();
    }

    #endregion
}
