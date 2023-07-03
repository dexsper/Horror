using System;
using System.Linq;
using FishNet;
using FishNet.Managing;
using FishNet.Managing.Scened;
using FishNet.Transporting;
using FishNet.Transporting.Multipass;
using FishNet.Transporting.Tugboat;
using UnityEngine;
using UnityEngine.UI;

public class SinglePlayerUI : MonoBehaviour
{
    [SerializeField] private CharactersData _characterData;
    [SerializeField] private Button _startButton,closeButton;

    [SerializeField] private GameObject authenticate, editName,gameAdObject;

    private Multipass _multipassTransport;
    private NetworkManager _networkManager;

    public static SinglePlayerUI Instance { get; private set; }


    private void Awake()
    {
        _networkManager = InstanceFinder.NetworkManager;
        _startButton.onClick.AddListener(StartSingleGame);
        closeButton.onClick.AddListener(CloseMenu);

        Instance = this;
    }

    private void OnEnable()
    {
        _networkManager.ServerManager.OnServerConnectionState += OnServerConnectionState;
        _multipassTransport = InstanceFinder.TransportManager.Transport as Multipass;
    }

    private void OnDisable()
    {
        _networkManager.ServerManager.OnServerConnectionState -= OnServerConnectionState;
    }

    private void OnServerConnectionState(ServerConnectionStateArgs args)
    {
        if (args.ConnectionState == LocalConnectionState.Started)
        {
            SceneLoadData sld = new SceneLoadData(MapWindowUI.Instance.SelectedMap);
            sld.ReplaceScenes = ReplaceOption.All;

            InstanceFinder.SceneManager.LoadGlobalScenes(sld);

            _multipassTransport.StartConnection(false, 0);
        }
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void StartSingleGame()
    {
        Destroy(_networkManager.gameObject.GetComponent<UnityServicesAuthenticator>());

        var fakePlayer = _networkManager.gameObject.AddComponent<FakePlayer>();
        
        fakePlayer.PlayerName = EditPlayerName.Instance.GetPlayerName();
        fakePlayer.CharacterName = _characterData.Characters.Keys.ElementAt(0);

        _multipassTransport.SetClientTransport(0);
        _multipassTransport.StartConnection(true, 0);
    }

    
    private void CloseMenu()
    {
        gameObject.SetActive(false);
        gameAdObject.SetActive(true);
        authenticate.SetActive(true);
        editName.SetActive(true);
    }
}
