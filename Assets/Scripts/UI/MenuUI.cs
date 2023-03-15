using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{

    #region Singleton
    
    public static MenuUI Instance;
    private void Awake()
    {
        _launcher = GetComponent<Launcher>();
    }
    #endregion
    
    [field: SerializeField] public Room RoomMenu { get; private set; }

    [SerializeField] private Transform createdRoomMenu, findGameMenu;
    
    [Range(4,12)]
    [SerializeField] private int roomNameMinLength, roomNameMaxLength;

    [SerializeField] private InputField inputField;
    
    [SerializeField] private float animationDuration;

    private Launcher _launcher;

    #region OpenCloseMenues

    public void OpenCreatedRoomMenu()
    {
        OpenMenu(createdRoomMenu);
    }

    public void CloseCreatedRoomMenu()
    {
        CloseMenu(createdRoomMenu);
    }
    
    public void OpenFindGameMenu()
    {
        OpenMenu(findGameMenu);
    }

    public void CloseFindGameMenu()
    {
        CloseMenu(findGameMenu);
    }
    
    public void OpenMenu(Transform menu)
    {
        menu.DOScale(1, animationDuration).From(0).SetEase(Ease.Linear);
    }

    public void CloseMenu(Transform menu)
    {
        menu.DOScale(0, animationDuration).From(1).SetEase(Ease.Linear);
    }
    #endregion

    private void Start()
    {
        inputField.characterLimit = roomNameMaxLength;
    }

    public void CreateRoom()
    {
        if (inputField.text != null)
        {
            if (inputField.text.ToCharArray().Length < roomNameMinLength)
            {
                Debug.Log($"Room name size must be more than {roomNameMinLength} letters and less than {roomNameMaxLength} letters");
            }
            else
            {
                _launcher.CreateRoom(inputField.text); 
                RoomMenu.OpenRoom(inputField.text);
            }
        }
    }
}
