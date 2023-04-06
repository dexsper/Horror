using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class LobbyListUI : MonoBehaviour {


    public static LobbyListUI Instance { get; private set; }



    [SerializeField] private Transform lobbySingleTemplate;
    [SerializeField] private Transform container;
    [SerializeField] private Button refreshButton;
    [SerializeField] private Button createLobbyButton;
 
    private LobbyManager _lobbyManager;


    private void Awake() {
        Instance = this;
        _lobbyManager = LobbyManager.Instance;

        lobbySingleTemplate.gameObject.SetActive(false);

        refreshButton.onClick.AddListener(RefreshButtonClick);
        createLobbyButton.onClick.AddListener(CreateLobbyButtonClick);
    }

    private void Start() {
        _lobbyManager.OnLobbyListChanged += LobbyManager_OnLobbyListChanged;
        _lobbyManager.OnJoinedLobby += LobbyManager_OnJoinedLobby;
        _lobbyManager.OnLeftLobby += LobbyManager_OnLeftLobby;
        _lobbyManager.OnKickedFromLobby += LobbyManager_OnKickedFromLobby;
    }

    private void LobbyManager_OnKickedFromLobby(object sender, LobbyEventArgs e) {
        Show();
    }

    private void LobbyManager_OnLeftLobby(object sender, EventArgs e) {
        Show();
    }

    private void LobbyManager_OnJoinedLobby(object sender, LobbyEventArgs e) {
        Hide();
    }

    private void LobbyManager_OnLobbyListChanged(object sender, OnLobbyListChangedEventArgs e) {
        UpdateLobbyList(e.lobbyList);
    }

    private void UpdateLobbyList(List<Lobby> lobbyList) {
        foreach (Transform child in container) {
            if (child == lobbySingleTemplate) continue;

            Destroy(child.gameObject);
        }

        foreach (Lobby lobby in lobbyList) {
            Transform lobbySingleTransform = Instantiate(lobbySingleTemplate, container);
            lobbySingleTransform.gameObject.SetActive(true);
            LobbyListSingleUI lobbyListSingleUI = lobbySingleTransform.GetComponent<LobbyListSingleUI>();
            lobbyListSingleUI.UpdateLobby(lobby);
        }
    }

    private void RefreshButtonClick() {
        _lobbyManager.RefreshLobbyList();
    }

    private void CreateLobbyButtonClick() {
        LobbyCreateUI.Instance.Show();
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

    private void Show() {
        gameObject.SetActive(true);
    }

}