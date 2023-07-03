using System;
using UnityEngine;
using UnityEngine.UI;

public class AuthenticateUI : MonoBehaviour
{
    [SerializeField] private Button authenticateButton;
    [SerializeField] private Button singlePlayerButton;
    [SerializeField] private GameObject editPlayerName;
    [SerializeField] private MultiplayerWindow multiplayerWindow;
    [SerializeField] private GameObject gameAdObject;
    [SerializeField] private GameObject settingsButton;

    private void Awake()
    {
        authenticateButton.onClick.AddListener(Authenticate);
        singlePlayerButton.onClick.AddListener(SinglePlayer);
    }

    private void SinglePlayer()
    {
        SinglePlayerUI.Instance.gameObject.SetActive(true);
        Hide();
    }

    private void Authenticate()
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

    private void Hide()
    {
        editPlayerName.SetActive(false);
        gameAdObject.SetActive(false);
        gameObject.SetActive(false);
        settingsButton.SetActive(false);
    }
}