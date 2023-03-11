using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    [SerializeField] private Transform joinLobbyMenu, settingsMenu, shopMenu;

    [SerializeField] private Room roomMenu;
    //[SerializeField] private Button buttons;
    [SerializeField] private Button createRoomButton;
    [Range(4,12)]
    [SerializeField] private int roomNameMinLength, roomNameMaxLength;

    [SerializeField] private InputField inputField;
    
    [SerializeField] private float animationDuration;

    private Launcher _launcher;

    #region OpenCloseMenues

    private void Awake()
    {
        _launcher = GetComponent<Launcher>();
    }

    private void OpenMenu(Transform menu)
    {
        menu.DOScale(1, animationDuration).From(0).SetEase(Ease.Linear);
    }

    private void CloseMenu(Transform menu)
    {
        menu.DOScale(0, animationDuration).From(1).SetEase(Ease.Linear);
    }

    public void OpenJoinLobbyMenu()
    {
        OpenMenu(joinLobbyMenu);
    }

    public void CloseJoinLobbyMenu()
    {
        CloseMenu(joinLobbyMenu);
    }
    
    public void OpenSettingsMenu()
    {
        OpenMenu(settingsMenu);
    }

    public void CloseSettingsMenu()
    {
        CloseMenu(settingsMenu);
    }
    
    public void OpenShopMenu()
    {
        OpenMenu(shopMenu);
    }

    public void CloseShopMenu()
    {
        CloseMenu(shopMenu);
    }

    #endregion

    private void Start()
    {
        inputField.characterLimit = roomNameMaxLength;
        createRoomButton.onClick.AddListener(CreateRoom);
    }

    private void CreateRoom()
    {
        if (inputField.text != null)
        {
            if (inputField.text.ToCharArray().Length < roomNameMinLength)
            {
                Debug.Log($"Room name size may be more than {roomNameMinLength} letters and less than {roomNameMaxLength} letters");
            }
            else
            {
                _launcher.CreateRoom(inputField.text); 
                roomMenu.OpenRoom(inputField.text);
            }
        }
    }
}
