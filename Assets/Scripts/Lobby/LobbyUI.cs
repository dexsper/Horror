using FishNet;
using FishNet.Managing.Scened;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    public static LobbyUI Instance { get; private set; }


    [SerializeField] private Transform playerSingleTemplate;
    [SerializeField] private Transform container;
    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI playerCountText;
    [SerializeField] private TextMeshProUGUI lobbyMapNameText;
    [SerializeField] private Image lobbyMapImage;
    [SerializeField] private Button changeMarineButton;
    [SerializeField] private Button changeNinjaButton;
    [SerializeField] private Button changeZombieButton;
    [SerializeField] private Button leaveLobbyButton;

    [SerializeField] private Button startGameButton;

    private LobbyManager _lobbyManager;


    private void Awake()
    {
        Instance = this;
        _lobbyManager = LobbyManager.Instance;

        playerSingleTemplate.gameObject.SetActive(false);

        changeMarineButton.onClick.AddListener(() =>
        {
            _lobbyManager.UpdatePlayerCharacter(LobbyManager.PlayerCharacter.Marine);
        });
        changeNinjaButton.onClick.AddListener(() =>
        {
            _lobbyManager.UpdatePlayerCharacter(LobbyManager.PlayerCharacter.Ninja);
        });
        changeZombieButton.onClick.AddListener(() =>
        {
            _lobbyManager.UpdatePlayerCharacter(LobbyManager.PlayerCharacter.Zombie);
        });

        leaveLobbyButton.onClick.AddListener(() =>
        {
            _lobbyManager.LeaveLobby();
        });
        startGameButton.onClick.AddListener(() =>
        {
            SceneLoadData sld = new SceneLoadData("Game");
            sld.ReplaceScenes = ReplaceOption.All;

            InstanceFinder.SceneManager.LoadGlobalScenes(sld);
        });
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
        UpdateLobby();
    }

    private void UpdateLobby()
    {
        UpdateLobby(_lobbyManager.GetJoinedLobby());
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

        startGameButton.gameObject.SetActive(_lobbyManager.IsLobbyHost());
        lobbyNameText.text = lobby.Name;
        playerCountText.text = lobby.Players.Count + "/" + lobby.MaxPlayers;
        lobbyMapNameText.text = lobby.Data[LobbyManager.KEY_MAP_NAME].Value;

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