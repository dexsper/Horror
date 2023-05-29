using FishNet;
using FishNet.Managing;
using FishNet.Managing.Scened;
using Sirenix.OdinInspector;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    public static LobbyUI Instance { get; private set; }

    [Title("References")]
    [SerializeField] private Transform playerSingleTemplate;
    [SerializeField] private Transform container;

    [Title("Text")]
    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI playerCountText;

    [Title("Buttons")]
    [SerializeField] private Button leaveLobbyButton;
    [SerializeField] private Button _startButton;

    [SerializeField] private MapWindowUI mapWindowUI;

    private LobbyManager _lobbyManager;
    private NetworkManager _networkManager;

    private void Awake()
    {
        Instance = this;

        _startButton.gameObject.SetActive(false);
        playerSingleTemplate.gameObject.SetActive(false);

        _lobbyManager = LobbyManager.Instance;
        _networkManager = InstanceFinder.NetworkManager;

        leaveLobbyButton.onClick.AddListener(() =>
        {
            _lobbyManager.LeaveLobby();
        });
        
        _startButton.onClick.AddListener(() =>
        {
            SceneLoadData sld = new SceneLoadData(LobbyManager.Instance.GetMap());
            sld.ReplaceScenes = ReplaceOption.All;

            InstanceFinder.SceneManager.LoadGlobalScenes(sld);
        });
    }

    public void EnableStartGameButton()
    {
        if(_startButton != null)
            _startButton.gameObject.SetActive(true);
    }

    public void DisableStartGameButton()
    {
        if(_startButton != null)
            _startButton.gameObject.SetActive(false);
    }

    private void Start()
    {
        _lobbyManager.OnJoinedLobby += UpdateLobby_Event;
        _lobbyManager.OnJoinedLobbyUpdate += UpdateLobby_Event;
        _lobbyManager.OnLeftLobby += LobbyManager_OnLeftLobby;
        _lobbyManager.OnKickedFromLobby += LobbyManager_OnLeftLobby;

        Hide();
    }

    private void OnDestroy()
    {
        _lobbyManager.OnJoinedLobby -= UpdateLobby_Event;
        _lobbyManager.OnJoinedLobbyUpdate -= UpdateLobby_Event;
        _lobbyManager.OnLeftLobby -= LobbyManager_OnLeftLobby;
        _lobbyManager.OnKickedFromLobby -= LobbyManager_OnLeftLobby;
    }

    private void LobbyManager_OnLeftLobby(object sender, System.EventArgs e)
    {
        ClearLobby();
        Hide();
    }

    private void UpdateLobby_Event(object sender, LobbyEventArgs e)
    {
        UpdateLobby(_lobbyManager.JoinedLobby);
    }

    private void UpdateLobby(Lobby lobby)
    {
        ClearLobby();

        foreach (Player player in lobby.Players)
        {
            Transform playerSingleTransform = Instantiate(playerSingleTemplate, container);
            playerSingleTransform.gameObject.SetActive(true);

            LobbyPlayerSingleUI lobbyPlayerSingleUI = playerSingleTransform.GetComponent<LobbyPlayerSingleUI>();

            lobbyPlayerSingleUI.SetKickPlayerButtonVisible(
                _lobbyManager.IsLobbyHost() &&
                player.Id != AuthenticationService.Instance.PlayerId
            );

            lobbyPlayerSingleUI.UpdatePlayer(player);
        }

        lobbyNameText.text = lobby.Name;
        playerCountText.text = lobby.Players.Count + "/" + lobby.MaxPlayers;

        mapWindowUI.UpdateMap(lobby.Data[LobbyManager.KEY_MAP_NAME].Value);
        _startButton.gameObject.SetActive(_lobbyManager.IsLobbyHost() && _networkManager.IsHost);

        Show();
    }

    private void ClearLobby()
    {
        foreach (Transform child in container)
        {
            if (child == playerSingleTemplate) continue;
            Destroy(child.gameObject);
        }
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

}