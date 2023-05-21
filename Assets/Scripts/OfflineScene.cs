using System.IO;
using FishNet.Managing;
using FishNet.Transporting;
using FishNet.Utility;
using UnityEditor.SearchService;
using UnityEngine;

using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

public class OfflineScene : MonoBehaviour
{
    [SerializeField, Scene] private string _offlineScene;

    private NetworkManager _networkManager;

    private void Awake()
    {
        InitializeOnce();
    }

    private void OnDestroy()
    {

        if (!ApplicationState.IsQuitting() && _networkManager != null && _networkManager.Initialized)
        {
            _networkManager.ClientManager.OnClientConnectionState -= ClientManager_OnClientConnectionState;
            _networkManager.ServerManager.OnServerConnectionState -= ServerManager_OnServerConnectionState;
        }
    }

    private void InitializeOnce()
    {
        _networkManager = GetComponentInParent<NetworkManager>();

        if (_networkManager == null)
        {
            NetworkManager.StaticLogError($"NetworkManager not found on {gameObject.name} or any parent objects. DefaultScene will not work.");
            return;
        }

        if (!_networkManager.Initialized)
            return;

        if (_offlineScene == string.Empty)
        {

            NetworkManager.StaticLogWarning("Offline scene is not specified. Default scenes will not load.");
            return;
        }

        _networkManager.ClientManager.OnClientConnectionState += ClientManager_OnClientConnectionState;
        _networkManager.ServerManager.OnServerConnectionState += ServerManager_OnServerConnectionState;
    }


    private void ServerManager_OnServerConnectionState(ServerConnectionStateArgs obj)
    {
        if (obj.ConnectionState == LocalConnectionState.Stopped)
        {
            LoadOfflineScene();
        }
    }

    private void ClientManager_OnClientConnectionState(ClientConnectionStateArgs obj)
    {
        if (obj.ConnectionState == LocalConnectionState.Stopped)
        {
            if (!_networkManager.IsServer)
                LoadOfflineScene();
        }
    }

    private void LoadOfflineScene()
    {
        if (UnitySceneManager.GetActiveScene().name == GetSceneName(_offlineScene))
            return;

        UnitySceneManager.LoadScene(_offlineScene);
    }

    private string GetSceneName(string fullPath)
    {
        return Path.GetFileNameWithoutExtension(fullPath);
    }
}
