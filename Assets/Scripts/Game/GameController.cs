using FishNet;
using FishNet.Managing;
using FishNet.Managing.Scened;
using FishNet.Object;


public class GameController : NetworkBehaviour
{
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

    private void Awake()
    {
        Instance = this;

        _spawner = GetComponent<PlayerSpawner>();
        _networkManager = InstanceFinder.NetworkManager;
        _sceneManager = InstanceFinder.SceneManager;

        _sceneManager.OnClientPresenceChangeEnd += OnClientJoinGame;
    }

    [Server]
    private void OnClientJoinGame(ClientPresenceChangeEventArgs args)
    {
        if (args.Scene != gameObject.scene) return;

        if (PlayerBehavior.Players.Exists(p => p.Owner == args.Connection))
            return;

        _spawner.SpawnPlayer(args.Connection);
    }
}
