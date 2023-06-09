using System;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyListUI : MonoBehaviour
{
    public static LobbyListUI Instance { get; private set; }

    [Title("References")]
    [SerializeField] private LobbyListSingleUI _lobbyTemplate;
    [SerializeField] private Transform _lobbyContainer;
    [SerializeField] private List<GameObject> objectsToDisable;

    [Title("Buttons")]
    [SerializeField] private Button refreshButton;
    [SerializeField] private Button createLobbyButton;
    [SerializeField] private Button closeButton;
    
    private LobbyManager _lobbyManager;


    private void Awake()
    {
        Instance = this;

        _lobbyManager = LobbyManager.Instance;
        _lobbyTemplate.gameObject.SetActive(false);

        refreshButton.onClick.AddListener(RefreshButtonClick);
        createLobbyButton.onClick.AddListener(CreateLobbyButtonClick);
        closeButton.onClick.AddListener(CloseWindow);
    }

    private void CloseWindow()
    {
        gameObject.SetActive(false);
        for (int i = 0; i < objectsToDisable.Count; i++)
        {
            objectsToDisable[i].transform.DOScale(1f, 0.2f).SetEase(Ease.Linear);
        }
    }
    
    private void Start()
    {
        _lobbyManager.OnLobbyListChanged += LobbyManager_OnLobbyListChanged;
        _lobbyManager.OnJoinedLobby += LobbyManager_OnJoinedLobby;
        _lobbyManager.OnLeftLobby += LobbyManager_OnLeftLobby;
        _lobbyManager.OnKickedFromLobby += LobbyManager_OnKickedFromLobby;

        CharacterWindowUI.Instance.OnCharacterSelected += SetActive;

        gameObject.SetActive(false);
    }

    private void SetActive()
    {
        //gameObject.SetActive(true);

        LobbyManager.Instance.RefreshLobbyList();
    }

    private void OnDestroy()
    {
        _lobbyManager.OnLobbyListChanged -= LobbyManager_OnLobbyListChanged;
        _lobbyManager.OnJoinedLobby -= LobbyManager_OnJoinedLobby;
        _lobbyManager.OnLeftLobby -= LobbyManager_OnLeftLobby;
        _lobbyManager.OnKickedFromLobby -= LobbyManager_OnKickedFromLobby;
        CharacterWindowUI.Instance.OnCharacterSelected -= SetActive;
    }

    private void LobbyManager_OnKickedFromLobby(object sender, LobbyEventArgs e)
    {
        Show();
    }

    private void LobbyManager_OnLeftLobby(object sender, EventArgs e)
    {
        Show();
    }

    private void LobbyManager_OnJoinedLobby(object sender, LobbyEventArgs e)
    {
        Hide();
        //_lobbyManager.UpdatePlayerCharacter(_characterWindow.SelectedCharacterName);
    }

    private void LobbyManager_OnLobbyListChanged(object sender, OnLobbyListChangedEventArgs e)
    {
        UpdateLobbyList(e.lobbyList);
    }

    private void UpdateLobbyList(List<Lobby> lobbyList)
    {
        foreach (Transform child in _lobbyContainer) {
            if (child == _lobbyContainer) continue;

            Destroy(child.gameObject);
        }

        foreach (Lobby lobby in lobbyList)
        {
            Transform lobbySingleTransform = Instantiate(_lobbyTemplate.transform, _lobbyContainer);
            lobbySingleTransform.gameObject.SetActive(true);
            LobbyListSingleUI lobbyListSingleUI = lobbySingleTransform.GetComponent<LobbyListSingleUI>();
            lobbyListSingleUI.UpdateLobby(lobby);
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
        gameObject.transform.DOScale(0f, 0.2f).SetEase(Ease.Linear);
    }

    private void Show()
    {
        gameObject.transform.DOScale(1.85f, 0.2f).SetEase(Ease.Linear);
    }

}