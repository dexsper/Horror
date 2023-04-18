using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyListUI : MonoBehaviour
{
    public static LobbyListUI Instance { get; private set; }

    [Title("References")]
    [SerializeField] private CharacterWindowUI _characterWindow;
    [SerializeField] private LobbyListSingleUI _lobbyTemplate;
    [SerializeField] private Transform _lobbyContainer;

    [Title("Buttons")]
    [SerializeField] private Button refreshButton;
    [SerializeField] private Button createLobbyButton;

    private LobbyManager _lobbyManager;


    private void Awake()
    {
        Instance = this;
        _lobbyManager = LobbyManager.Instance;

        _lobbyTemplate.gameObject.SetActive(false);

        refreshButton.onClick.AddListener(RefreshButtonClick);
        createLobbyButton.onClick.AddListener(CreateLobbyButtonClick);
    }

    private void Start()
    {
        _lobbyManager.OnLobbyListChanged += LobbyManager_OnLobbyListChanged;
        _lobbyManager.OnJoinedLobby += LobbyManager_OnJoinedLobby;
        _lobbyManager.OnLeftLobby += LobbyManager_OnLeftLobby;
        _lobbyManager.OnKickedFromLobby += LobbyManager_OnKickedFromLobby;
    }

    private void LobbyManager_OnKickedFromLobby(object sender, LobbyEventArgs e)
    {
        Show();
    }

    private void LobbyManager_OnLeftLobby(object sender, EventArgs e)
    {
        _characterWindow.gameObject.SetActive(true);

        Show();
    }

    private void LobbyManager_OnJoinedLobby(object sender, LobbyEventArgs e)
    {
        Hide();

        _characterWindow.gameObject.SetActive(false);
        _lobbyManager.UpdatePlayerCharacter(_characterWindow.SelectedCharacterName);
    }

    private void LobbyManager_OnLobbyListChanged(object sender, OnLobbyListChangedEventArgs e)
    {
        UpdateLobbyList(e.lobbyList);
    }

    private void UpdateLobbyList(List<Lobby> lobbyList)
    {
        foreach (Transform child in _lobbyContainer)
        {
            if (child == _lobbyTemplate) continue;

            //Destroy(child.gameObject);
        }

        foreach (Lobby lobby in lobbyList)
        {
            LobbyListSingleUI lobbyInstance = Instantiate(_lobbyTemplate, _lobbyContainer);

            lobbyInstance.UpdateLobby(lobby);
            lobbyInstance.gameObject.SetActive(true);
        }
    }

    private void RefreshButtonClick()
    {
        _lobbyManager.RefreshLobbyList();
    }

    private void CreateLobbyButtonClick()
    {
        LobbyCreateUI.Instance.Show();
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