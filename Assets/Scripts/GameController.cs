using FishNet;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Managing.Scened;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Utility;
using UnityEngine;

public class GameController : NetworkBehaviour
{
    [SerializeField, Scene] private string _deathRoomScene;

    private static GameController _instance;

    private PlayerSpawner _spawner;
    private SceneManager _sceneManager;
    private NetworkManager _networkManager;

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
    [field: SyncVar] public bool GameFinished { get; private set; }

    private void Awake()
    {
        Instance = this;

        _spawner = GetComponent<PlayerSpawner>();
        _networkManager = InstanceFinder.NetworkManager;
        _sceneManager = InstanceFinder.SceneManager;
    }

    [Server]
    private void Start()
    {
        if (base.IsServer)
        {
            DeathRoom.OnPlayerLeave += OnPlayerLeaveDeathRoom;
            PlayerBehavior.OnDead += OnPlayerDead;
            Generator.OnRepaired += OnGeneratorRepaired;

            _sceneManager.OnClientPresenceChangeEnd += OnClientPresenceChangeEnd;
            _sceneManager.LoadGlobalScenes(new SceneLoadData(_deathRoomScene));
        }
    }

    [Server]
    private void OnGeneratorRepaired(Generator generator)
    {
        for (int i = 0; i < Generator.Generators.Count; i++)
        {
            if (!Generator.Generators[i].IsRepaired)
                return;
        }

        foreach(var zone in LeaveZone.LeaveZones)
        {
            zone.Activate();
        }
    }

    [Server]
    private void OnPlayerLeaveDeathRoom(PlayerBehavior player)
    {
        player.Health.Restore();

        _spawner.RespawnPlayer(player.NetworkObject);
    }

    [Server]
    private void OnPlayerDead(PlayerBehavior player)
    {
        DeathRoom.Instance.AddPlayer(player);
    }

    [Server]
    private void OnClientPresenceChangeEnd(ClientPresenceChangeEventArgs args)
    {
        if (args.Scene != gameObject.scene || !args.Added) return;

        OnClientJoin(args.Connection);
    }

    [Server]
    private void OnClientJoin(NetworkConnection connection)
    {
        if (PlayerBehavior.Players.Exists(p => p.Owner == connection))
            return;

        _spawner.SpawnPlayer(connection);
    }
}
