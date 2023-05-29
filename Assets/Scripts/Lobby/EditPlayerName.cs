using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class EditPlayerName : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private Transform inputKeyBoard;

    [SerializeField] private Vector3 startPosition, endPosition;
    
    private string playerName = "Player";

    private bool _isOpened;
    
    public static EditPlayerName Instance { get; private set; }
    public event EventHandler OnNameChanged;

    private void Awake()
    {
        Instance = this;

        GetComponent<Button>().onClick.AddListener(OnButtonClick);
        GetComponent<Button>().onClick.AddListener(() =>
        {
            UI_InputWindow.Show_Static("Player Name", "", "abcdefghijklmnopqrstuvxywzABCDEFGHIJKLMNOPQRSTUVXYWZ .,-", 20,
            () =>
            {
                CloseKeyboard();
            },
            (string newName) =>
            {
                playerName = newName;

                playerNameText.text = playerName;

                OnNameChanged?.Invoke(this, EventArgs.Empty);
                CloseKeyboard();
            });
        });

        playerNameText.text = playerName;
    }

    private void OnButtonClick()
    {
        if (!_isOpened)
        {
            OpenKeyBoard();
        }
        else
        {
            CloseKeyboard();
        }
    }

    private void OpenKeyBoard()
    {
        inputKeyBoard.DOLocalMoveY(startPosition.y, 0.25f).SetEase(Ease.Linear);
        _isOpened = true;
    }

    private void CloseKeyboard()
    {
        inputKeyBoard.DOLocalMoveY(endPosition.y, 0.25f).SetEase(Ease.Linear);
        _isOpened = false;
    }

    private void Start()
    {
        OnNameChanged += EditPlayerName_OnNameChanged;
    }

    private void EditPlayerName_OnNameChanged(object sender, EventArgs e)
    {
        LobbyManager.Instance.UpdatePlayerName(GetPlayerName());
    }

    public string GetPlayerName()
    {
        return playerName;
    }


}