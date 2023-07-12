using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class AuthenticateUI : MonoBehaviour
{
    [SerializeField] private Button authenticateButton;
    [SerializeField] private Button singlePlayerButton;
    [SerializeField] private GameObject editPlayerName;
    [SerializeField] private MultiplayerWindow multiplayerWindow;
    [SerializeField] private SinglePlayerUI singlePlayerWindow;
    [SerializeField] private GameObject gameAdObject;
    [SerializeField] private GameObject settingsButton;

    private void Awake()
    {
        authenticateButton.onClick.AddListener(Authenticate);
        singlePlayerButton.onClick.AddListener(SinglePlayer);
    }

    private void SinglePlayer()
    {
        SinglePlayerUI.Instance.OpenMenu();
        Hide();
    }

    private void Authenticate()
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            if (LobbyManager.Instance.IsAuthenticated)
            {
                PlayerEconomy.Instance.Refresh();

                Hide();
                multiplayerWindow.OpenMenu();
                return;
            }
        
            LobbyManager.Instance.Authenticate(EditPlayerName.Instance.GetPlayerName());
            multiplayerWindow.OpenMenu();
            Hide(); 
        }
        else
        {
            SinglePlayer();
        }
    }

    private void Hide()
    {
        editPlayerName.transform.DOScale(0f, 0.2f).SetEase(Ease.Linear);
        gameAdObject.SetActive(false);
        gameObject.transform.DOScale(0f, 0.2f).SetEase(Ease.Linear);
        settingsButton.transform.DOScale(0f, 0.2f).SetEase(Ease.Linear);
    }
}