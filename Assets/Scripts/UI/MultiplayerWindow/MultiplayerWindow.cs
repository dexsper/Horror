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

    [SerializeField] private TextMeshProUGUI quickPlayText;

    [SerializeField] private CharacterWindowUI characterWindowUI;
    
    [SerializeField] private GameObject lobbyCreateUI;
    
    [SerializeField] private GameObject authentificateObject, editNameObject;

    [SerializeField] private GameObject tutorialObject,settingsMenuButton,settingsMpMenuButton;

    [SerializeField] private List<GameObject> objectsToDisable;

    private float _timerCounter, _timer = 2f;
    private bool _timerStart;
    private void Awake()
    {
        openCharacterWindowButton.onClick.AddListener(OpenCharacterWindow);
        openLobbyWindowButton.onClick.AddListener(OpenLobbyWindow);
        quickGameButton.onClick.AddListener(QuickGame);
        closeButton.onClick.AddListener(CloseMenu);

        LobbyManager.OnCantFindOpenLobby += OnCantFindOpenLobby;
        characterWindowUI.OnCharacterSelected += OnCharacterAction;
        
        settingsMpMenuButton.transform.DOScale(0f, 0.2f).SetEase(Ease.Linear);
        
        CloseQuickText();
        
        OpenCharacterWindow();
        openCharacterWindowButton.transform.DOScale(0f, 0.2f).SetEase(Ease.Linear);
    }

    private void CheckForFirstEntry()
    {
        if (PlayerPrefs.HasKey("PlayedOnce"))
        {
            tutorialObject.SetActive(true);
        }
        else
        {
            tutorialObject.SetActive(false);
            PlayerPrefs.SetInt("PlayedOnce",1);
        }
        
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
        transform.DOScale(1f, 0.2f).SetEase(Ease.Linear);

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
