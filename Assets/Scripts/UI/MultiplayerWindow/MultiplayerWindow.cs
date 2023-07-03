using System;
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

    [SerializeField] private GameObject lobbyCreateUI, characterWindowUI;
    
    [SerializeField] private GameObject authentificateObject, editNameObject;

    [SerializeField] private GameObject tutorialObject;

    private float _timerCounter, _timer = 2f;
    private bool _timerStart;
    private void Awake()
    {
        openCharacterWindowButton.onClick.AddListener(OpenCharacterWindow);
        openLobbyWindowButton.onClick.AddListener(OpenLobbyWindow);
        quickGameButton.onClick.AddListener(QuickGame);
        closeButton.onClick.AddListener(CloseMenu);
        
        openLobbyWindowButton.interactable = false;
        quickGameButton.interactable = false;

        CharacterWindowUI.Instance.OnCharacterSelected += OnCharacterSelected;
        LobbyManager.OnCantFindOpenLobby += OnCantFindOpenLobby;
        
        CloseQuickText();
    }

    private void CheckForFirstEntry()
    {
        if (PlayerPrefs.HasKey("FirstPlay"))
        {
            openCharacterWindowButton.gameObject.SetActive(true);
            openLobbyWindowButton.gameObject.SetActive(true);
            openLobbyWindowButton.interactable = true;
            openCharacterWindowButton.interactable = true;
            quickGameButton.interactable = true;
            Debug.Log("HasKey");
            tutorialObject.SetActive(true);
            if (PlayerPrefs.HasKey("TutorialShowed"))
            {
                tutorialObject.SetActive(false);
            }
            else
            {
                PlayerPrefs.SetInt("TutorialShowed",1);
                
            }
        }
        else
        {
            PlayerPrefs.SetInt("FirstPlay",1);
            openCharacterWindowButton.gameObject.SetActive(false);
            openLobbyWindowButton.gameObject.SetActive(false);
            quickGameButton.interactable = true;
            tutorialObject.SetActive(false);
            Debug.Log("HasNoKey");
        }
    }

    [ContextMenu("Clear Prefs")]
    private void ClearPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
    
    private void OnDisable()
    {
        CharacterWindowUI.Instance.OnCharacterSelected -= OnCharacterSelected;
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

    private void OnCharacterSelected()
    {
        openLobbyWindowButton.interactable = true;
        quickGameButton.interactable = true;
    }
    
    public void OpenMenu()
    {
        gameObject.SetActive(true);
        
        authentificateObject.SetActive(false);
        editNameObject.SetActive(false);
        
        CheckForFirstEntry();
    }

    public void CloseMenu()
    {
        gameObject.SetActive(false);
        
        authentificateObject.SetActive(true);
        editNameObject.SetActive(true);
    }
    
    private void OpenCharacterWindow()
    {
        characterWindowUI.gameObject.SetActive(true);
    }

    private void OpenLobbyWindow()
    {
        lobbyCreateUI.SetActive(true);
    }

    private void QuickGame()
    {
        LobbyManager.Instance.QuickJoinLobby();
    }
}
