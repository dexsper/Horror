using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerWindow : MonoBehaviour
{
    [SerializeField] private Button openCharacterWindowButton;
    [SerializeField] private Button openLobbyWindowButton;
    [SerializeField] private Button quickGameButton;

    [SerializeField] private Button closeButton;

    [SerializeField] private CharacterWindowUI characterWindowUI;
    
    [SerializeField] private GameObject lobbyCreateUI;
    
    [SerializeField] private GameObject authentificateObject, editNameObject;

    [SerializeField] private GameObject settingsMenuButton,settingsMpMenuButton,closeCharactersButton;

    [SerializeField] private List<GameObject> objectsToDisable;

    private string lobbyName = "OpenLobby";
    private bool isPrivate;
    private int maxPlayers = 4;
    
    private void Awake()
    {
        openCharacterWindowButton.onClick.AddListener(OpenCharacterWindow);
        openLobbyWindowButton.onClick.AddListener(OpenLobbyWindow);
        quickGameButton.onClick.AddListener(QuickGame);
        closeButton.onClick.AddListener(CloseMenu);

        LobbyManager.OnCantFindOpenLobby += OnCantFindOpenLobby;
        characterWindowUI.OnCharacterSelected += OnCharacterAction;
        
        settingsMpMenuButton.transform.DOScale(0f, 0.2f).SetEase(Ease.Linear);

        OpenCharacterWindow();
        openCharacterWindowButton.transform.DOScale(0f, 0.2f).SetEase(Ease.Linear);
    }

    private void CheckForFirstEntry()
    {
        quickGameButton.transform.DOScale(0f, 0.2f).SetEase(Ease.Linear);
        openLobbyWindowButton.transform.DOScale(0f, 0.2f).SetEase(Ease.Linear);
    }

    private void OnCharacterAction()
    {
        quickGameButton.transform.DOScale(1f, 0.2f).SetEase(Ease.Linear);
        openLobbyWindowButton.transform.DOScale(1f, 0.2f).SetEase(Ease.Linear);
    }
    
    [ContextMenu("Clear Prefs")]
    private void ClearPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
    
    private void OnDestroy()
    {
        LobbyManager.OnCantFindOpenLobby -= OnCantFindOpenLobby;
        characterWindowUI.OnCharacterSelected -= OnCharacterAction;
    }

    private void OnCantFindOpenLobby()
    {
        LobbyManager.Instance.CreateLobby(
            lobbyName,
            maxPlayers,
            isPrivate
        );
    }

    public void OpenMenu()
    {
        transform.DOScale(1f, 0.2f).SetEase(Ease.Linear);

        closeCharactersButton.SetActive(false);
        settingsMenuButton.transform.DOScale(0f, 0.2f).SetEase(Ease.Linear);
        settingsMpMenuButton.transform.DOScale(1f, 0.2f).SetEase(Ease.Linear);

        authentificateObject.transform.DOScale(0f, 0.2f).SetEase(Ease.Linear);
        editNameObject.transform.DOScale(0f, 0.2f).SetEase(Ease.Linear);
        
        CheckForFirstEntry();
    }

    private void CloseMenu()
    {
        transform.DOScale(0f, 0.2f).SetEase(Ease.Linear);
        
        settingsMenuButton.transform.DOScale(1f, 0.2f).SetEase(Ease.Linear);
        settingsMpMenuButton.transform.DOScale(0f, 0.2f).SetEase(Ease.Linear);
        
        authentificateObject.transform.DOScale(1f, 0.2f).SetEase(Ease.Linear);
        editNameObject.transform.DOScale(1f, 0.2f).SetEase(Ease.Linear);
    }
    
    private void OpenCharacterWindow()
    {
        characterWindowUI.OpenWindow();
        for (int i = 0; i < objectsToDisable.Count; i++)
        {
            objectsToDisable[i].transform.DOScale(0f, 0.2f).SetEase(Ease.Linear);
        }
    }

    private void OpenLobbyWindow()
    {
        lobbyCreateUI.SetActive(true);
        for (int i = 0; i < objectsToDisable.Count; i++)
        {
            objectsToDisable[i].transform.DOScale(0f, 0.2f).SetEase(Ease.Linear);
        }
    }

    private void QuickGame()
    {
        LobbyManager.Instance.QuickJoinLobby();
        for (int i = 0; i < objectsToDisable.Count; i++)
        {
            objectsToDisable[i].transform.DOScale(0f, 0.2f).SetEase(Ease.Linear);
        }
    }
}
