using System;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    public const string KEY_PLAYER_NAME = "PlayerName";
    public const string KEY_PLAYER_CHARACTER = "Character";
    public const string KEY_CONNECTION_CODE = "ConnectionJoinCode";
    public const string KEY_MAP_NAME = "MapName";

    private static LobbyManager _instance;
    private float _heartbeatTimer;
    private float _lobbyPollTimer;
    private float _refreshLobbyListTimer = 5f;
    private string _playerName;
    private string _mapName;


    public Lobby JoinedLobby;
    public event EventHandler OnLeftLobby;
    public event EventHandler<LobbyEventArgs> OnJoinedLobby;
    public event EventHandler<LobbyEventArgs> OnJoinedLobbyUpdate;
    public event EventHandler<LobbyEventArgs> OnKickedFromLobby;
    public event EventHandler<OnLobbyListChangedEventArgs> OnLobbyListChanged;
    public static LobbyManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<LobbyManager>();
            }

            return _instance;
        }
    }

    public enum PlayerCharacter
    {
        Marine,
        Ninja,
        Zombie
    }


    private void Update()
    {
        //HandleRefreshLobbyList(); // Disabled Auto Refresh for testing with multiple builds
        HandleLobbyHeartbeat();
        HandleLobbyPolling();
    }

    public async void Authenticate(string playerName)
    {
        this._playerName = playerName;
        InitializationOptions initializationOptions = new InitializationOptions();
        initializationOptions.SetProfile(playerName);

        await UnityServices.InitializeAsync(initializationOptions);

        AuthenticationService.Instance.SignedIn += () =>
        {
            // do nothing
            Debug.Log("Signed in! " + AuthenticationService.Instance.PlayerId);

            RefreshLobbyList();
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private void HandleRefreshLobbyList()
    {
        if (UnityServices.State == ServicesInitializationState.Initialized && AuthenticationService.Instance.IsSignedIn)
        {
            _refreshLobbyListTimer -= Time.deltaTime;
            if (_refreshLobbyListTimer < 0f)
            {
                float refreshLobbyListTimerMax = 5f;
                _refreshLobbyListTimer = refreshLobbyListTimerMax;

                RefreshLobbyList();
            }
        }
    }

    private async void HandleLobbyHeartbeat()
    {
        if (IsLobbyHost())
        {
            _heartbeatTimer -= Time.deltaTime;
            if (_heartbeatTimer < 0f)
            {
                float heartbeatTimerMax = 15f;
                _heartbeatTimer = heartbeatTimerMax;

                Debug.Log("Heartbeat");
                await LobbyService.Instance.SendHeartbeatPingAsync(JoinedLobby.Id);
            }
        }
    }

    private async void HandleLobbyPolling()
    {
        if (JoinedLobby != null)
        {
            _lobbyPollTimer -= Time.deltaTime;
            if (_lobbyPollTimer < 0f)
            {
                float lobbyPollTimerMax = 1.1f;
                _lobbyPollTimer = lobbyPollTimerMax;

                JoinedLobby = await LobbyService.Instance.GetLobbyAsync(JoinedLobby.Id);

                OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = JoinedLobby });

                if (!IsPlayerInLobby())
                {
                    // Player was kicked out of this lobby
                    Debug.Log("Kicked from Lobby!");

                    OnKickedFromLobby?.Invoke(this, new LobbyEventArgs { lobby = JoinedLobby });

                    JoinedLobby = null;
                }
            }
        }
    }

    public Lobby GetJoinedLobby()
    {
        return JoinedLobby;
    }

    public bool IsLobbyHost()
    {
        return JoinedLobby != null && JoinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    private bool IsPlayerInLobby()
    {
        if (JoinedLobby != null && JoinedLobby.Players != null)
        {
            foreach (Player player in JoinedLobby.Players)
            {
                if (player.Id == AuthenticationService.Instance.PlayerId)
                {
                    // This player is in this lobby
                    return true;
                }
            }
        }

        return false;
    }

    private Player GetPlayer()
    {
        return new Player(AuthenticationService.Instance.PlayerId, null, new Dictionary<string, PlayerDataObject>
        {
            {KEY_PLAYER_NAME, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, _playerName)},
            {
                KEY_PLAYER_CHARACTER,
                new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, PlayerCharacter.Marine.ToString())
            }
        });
    }

    public string GetMap()
    {
        return _mapName;
    }

    public async void CreateLobby(string lobbyName, int maxPlayers, bool isPrivate)
    {
        Player player = GetPlayer();

        CreateLobbyOptions options = new CreateLobbyOptions
        {
            Player = player,
            IsPrivate = isPrivate,
            Data = new Dictionary<string, DataObject>()
                {
                    {
                        KEY_CONNECTION_CODE, new DataObject(
                            visibility: DataObject.VisibilityOptions.Member,
                            value: "")
                    },
                    {
                        KEY_MAP_NAME,new DataObject(visibility: DataObject.VisibilityOptions.Public,
                        value: "Backrooms")
                    }
                }
        };

        Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);

        JoinedLobby = lobby;

        OnJoinedLobby?.Invoke(this, new LobbyEventArgs
        {
            lobby = lobby
        });

        Debug.Log("Created Lobby " + lobby.Name);
    }

    public async void UpdateLobbyMap(string mapName)
    {
        this._mapName = mapName;
        
        UpdateLobbyOptions options = new UpdateLobbyOptions();

        options.Data = new Dictionary<string, DataObject>()
        {
            {
                KEY_MAP_NAME, new DataObject(DataObject.VisibilityOptions.Public, mapName)
            }
        };

        Lobby lobby = await LobbyService.Instance.UpdateLobbyAsync(JoinedLobby.Id, options);
        JoinedLobby = lobby;
        
        OnJoinedLobbyUpdate?.Invoke(this,new LobbyEventArgs{lobby = JoinedLobby});
    }
    
    public async void RefreshLobbyList()
    {
        try
        {
            QueryLobbiesOptions options = new QueryLobbiesOptions();
            options.Count = 25;

            // Filter for open lobbies only
            options.Filters = new List<QueryFilter>
            {
                new QueryFilter(
                    field: QueryFilter.FieldOptions.AvailableSlots,
                    op: QueryFilter.OpOptions.GT,
                    value: "0")
            };

            // Order by newest lobbies first
            options.Order = new List<QueryOrder>
            {
                new QueryOrder(
                    asc: false,
                    field: QueryOrder.FieldOptions.Created)
            };

            QueryResponse lobbyListQueryResponse = await Lobbies.Instance.QueryLobbiesAsync();

            OnLobbyListChanged?.Invoke(this,
                new OnLobbyListChangedEventArgs { lobbyList = lobbyListQueryResponse.Results });
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void JoinLobbyByCode(string lobbyCode)
    {
        Player player = GetPlayer();

        Lobby lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, new JoinLobbyByCodeOptions
        {
            Player = player
        });

        JoinedLobby = lobby;

        OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });
    }

    public async void JoinLobby(Lobby lobby)
    {
        Player player = GetPlayer();

        JoinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id, new JoinLobbyByIdOptions
        {
            Player = player
        });

        OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });
    }

    public async void UpdatePlayerName(string playerName)
    {
        this._playerName = playerName;

        if (JoinedLobby != null)
        {
            try
            {
                UpdatePlayerOptions options = new UpdatePlayerOptions();

                options.Data = new Dictionary<string, PlayerDataObject>()
                {
                    {
                        KEY_PLAYER_NAME, new PlayerDataObject(
                            visibility: PlayerDataObject.VisibilityOptions.Public,
                            value: playerName)
                    }
                };

                string playerId = AuthenticationService.Instance.PlayerId;

                Lobby lobby = await LobbyService.Instance.UpdatePlayerAsync(JoinedLobby.Id, playerId, options);
                JoinedLobby = lobby;

                OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = JoinedLobby });
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    public async void UpdatePlayerCharacter(PlayerCharacter playerCharacter)
    {
        if (JoinedLobby != null)
        {
            try
            {
                UpdatePlayerOptions options = new UpdatePlayerOptions();

                options.Data = new Dictionary<string, PlayerDataObject>()
                {
                    {
                        KEY_PLAYER_CHARACTER, new PlayerDataObject(
                            visibility: PlayerDataObject.VisibilityOptions.Public,
                            value: playerCharacter.ToString())
                    }
                };

                string playerId = AuthenticationService.Instance.PlayerId;

                Lobby lobby = await LobbyService.Instance.UpdatePlayerAsync(JoinedLobby.Id, playerId, options);
                JoinedLobby = lobby;

                OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = JoinedLobby });
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    public async void UpdateConnectionCode(string code)
    {
        if (!IsLobbyHost() || JoinedLobby == null)
            return;

        try
        {
            UpdateLobbyOptions options = new UpdateLobbyOptions();

            options.Data = new Dictionary<string, DataObject>()
                {
                    {
                        KEY_CONNECTION_CODE, new DataObject(
                            visibility: DataObject.VisibilityOptions.Member,
                        value: code)
                    }
                };

            Lobby lobby = await LobbyService.Instance.UpdateLobbyAsync(JoinedLobby.Id, options);
            JoinedLobby = lobby;

            Debug.Log($"Connection code updated: {lobby.Data[KEY_CONNECTION_CODE].Value}");

            OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = JoinedLobby });
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void QuickJoinLobby()
    {
        try
        {
            QuickJoinLobbyOptions options = new QuickJoinLobbyOptions();

            Lobby lobby = await LobbyService.Instance.QuickJoinLobbyAsync(options);
            JoinedLobby = lobby;

            OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void LeaveLobby()
    {
        if (JoinedLobby != null)
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(JoinedLobby.Id, AuthenticationService.Instance.PlayerId);

                JoinedLobby = null;

                OnLeftLobby?.Invoke(this, EventArgs.Empty);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    public async void KickPlayer(string playerId)
    {
        if (IsLobbyHost())
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(JoinedLobby.Id, playerId);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }
}