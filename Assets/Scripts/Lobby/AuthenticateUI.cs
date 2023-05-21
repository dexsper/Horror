using UnityEngine;
using UnityEngine.UI;

public class AuthenticateUI : MonoBehaviour
{
    [SerializeField] private Button authenticateButton;

    private void Awake()
    {
        authenticateButton.onClick.AddListener(Authenticate);

        if (LobbyManager.Instance.IsAuthenticated)
        {
            LobbyManager.Instance.RefreshLobbyList();
            Hide();
        }
    }

    private void Authenticate()
    {
        LobbyManager.Instance.Authenticate(EditPlayerName.Instance.GetPlayerName());
        Hide();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}