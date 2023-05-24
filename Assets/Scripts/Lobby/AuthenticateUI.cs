using System;
using UnityEngine;
using UnityEngine.UI;

public class AuthenticateUI : MonoBehaviour
{
    [SerializeField] private Button authenticateButton;
    [SerializeField] private Button singlePlayerButton;
    [SerializeField] private GameObject editPlayerName;
    [SerializeField] private GameObject characterWindowUI;

    private void Awake()
    {
        authenticateButton.onClick.AddListener(Authenticate);
        singlePlayerButton.onClick.AddListener(SinglePlayer);

        if (LobbyManager.Instance.IsAuthenticated)
        {
            PlayerEconomy.Instance.Refresh();

            Hide();
        }
    }

    private void SinglePlayer()
    {
        SinglePlayerUI.Instance.gameObject.SetActive(true);
        Hide();
    }

    private void Authenticate()
    {
        LobbyManager.Instance.Authenticate(EditPlayerName.Instance.GetPlayerName());

        Hide();
        characterWindowUI.SetActive(true);
    }

    private void Hide()
    {
        editPlayerName.SetActive(false);
        gameObject.SetActive(false);
    }
}