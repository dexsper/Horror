using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerWindow : MonoBehaviour
{
    [SerializeField] private Button openCharacterWindowButton;
    [SerializeField] private Button openLobbyWindowButton;
    [SerializeField] private Button quickGameButton;

    [SerializeField] private Button closeButton;

    [SerializeField] private TextMeshProUGUI quickPlayText;

    [SerializeField] private CharacterWindowUI characterWindowUI;
    
    [SerializeField] private GameObject lobbyCreateUI;
    
    [SerializeField] private GameObject authentificateObject, editNameObject;

    [SerializeField] private GameObject tutorialObject,settingsMenuButton,settingsMpMenuButton;

    [SerializeField] private List<GameObject> objectsToDisable;

    private GameObject _openCharacters, _openLobby;
    
    private float _timerCounter, _timer = 2f;
    private bool _timerStart;
    private void Awake()
    {
        openCharacterWindowButton.onClick.AddListener(OpenCharacterWindow);
        openLobbyWindowButton.onClick.AddListener(OpenLobbyWindow);
        quickGameButton.onClick.AddListener(QuickGame);
        closeButton.onClick.AddListener(CloseMenu);

        LobbyManager.OnCantFindOpenLobby += OnCantFindOpenLobby;
        
        settingsMpMenuButton.SetActive(false);
        
        CloseQuickText();
    }

    private void CheckForFirstEntry()
    {
        if (PlayerPrefs.HasKey("PlayedOnce"))
        {
            tutorialObject.SetActive(true);
            _openCharacters.SetActive(true);
            _openLobby.SetActive(true);
        }
        else
        {
            tutorialObject.SetActive(false);
            _openCharacters.SetActive(false);
            _openLobby.SetActive(false);
            PlayerPrefs.SetInt("PlayedOnce",1);
        }
    }

    [ContextMenu("Clear Prefs")]
    private void ClearPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
    
    private void OnDestroy()
    {
        LobbyManager.OnCantFindOpenLobby -= OnCantFindOpenLobby;
    }

    private void OnCantFindOpenLobby()
    {
        quickPlayText.gameObject.SetActive(true);
        _timerStart = true;
        OpenLobbyWindow();
    }

    private void CloseQuickText()
    {
        quickPlayText.gameObject.SetActive(false);
        _timerStart = false;
        _timerCounter = 0f;
    }

    private void Update()
    {
        if (_timerStart)
        {
            _timerCounter += Time.deltaTime;
            if (_timerCounter >= _timer)
            {
                _timerCounter = _timer;
                CloseQuickText();
            }
        }
    }

    public void OpenMenu()
    {
        gameObject.SetActive(true);
        
        settingsMenuButton.SetActive(false);
        settingsMpMenuButton.SetActive(true);
        _openCharacters = openCharacterWindowButton.gameObject;
        _openLobby = openLobbyWindowButton.gameObject;
        
        authentificateObject.SetActive(false);
        editNameObject.SetActive(false);
        
        CheckForFirstEntry();
    }

    public void CloseMenu()
    {
        gameObject.SetActive(false);
        
        settingsMenuButton.SetActive(true);
        settingsMpMenuButton.SetActive(false);
        
        authentificateObject.SetActive(true);
        editNameObject.SetActive(true);
    }
    
    private void OpenCharacterWindow()
    {
        characterWindowUI.OpenWindow();
        for (int i = 0; i < objectsToDisable.Count; i++)
        {
            objectsToDisable[i].SetActive(false);
        }
    }

    private void OpenLobbyWindow()
    {
        lobbyCreateUI.SetActive(true);
        for (int i = 0; i < objectsToDisable.Count; i++)
        {
            objectsToDisable[i].SetActive(false);
        }
    }

    private void QuickGame()
    {
        LobbyManager.Instance.QuickJoinLobby();
    }
}
