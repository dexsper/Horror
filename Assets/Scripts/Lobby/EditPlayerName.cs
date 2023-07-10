using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class EditPlayerName : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private Transform inputKeyBoard;

    [SerializeField] private Vector3 startPosition, endPosition;
    
    private string playerName = "Player";

    private bool _isOpened;
    
    public static EditPlayerName Instance { get; private set; }
    public event EventHandler OnNameChanged;
    
    public static event Action OnNameEditOpen;
    public static event Action OnNameEditClose;

    private void Awake()
    {
        Instance = this;

        GetComponent<Button>().onClick.AddListener(OnButtonClick);
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

    public void ConfirmName(string newName)
    {
        if (newName == "Player" || newName == "PLAYER")
        {
            var index = Random.Range(100, 1000);

            newName = $"Player{index}";
            
            playerName = newName;

            playerNameText.text = playerName;

            OnNameChanged?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            playerName = newName;

            playerNameText.text = playerName;

            OnNameChanged?.Invoke(this, EventArgs.Empty);
        }
        CloseKeyboard();
    }
    
    private void OpenKeyBoard()
    {
        playerNameText.text = null;
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

    private void OnDestroy()
    {
        OnNameChanged -= EditPlayerName_OnNameChanged;
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