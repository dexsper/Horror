using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomListItem : MonoBehaviour
{
   [SerializeField] private TextMeshProUGUI roomNameText;
   [SerializeField] private TextMeshProUGUI roomPlayerCount;
   [SerializeField] private Button joinRoomButton;

   private RoomInfo _info;
   [field: SerializeField] public int maxPlayersCount { get; private set; } = 4; 

   private void Awake()
   {
      joinRoomButton.onClick.AddListener(OnClick);
   }
   
   public void SetUp(RoomInfo roomInfo)
   {
      _info = roomInfo;
      roomPlayerCount.text = $"{_info.PlayerCount}/{maxPlayersCount}";
      roomNameText.text = _info.Name;
   }

   private void OnClick()
   {
      MenuUI.Instance.OpenCreatedRoomMenu();
      MenuUI.Instance.CloseFindGameMenu();
      Launcher.Instance.JoinRoom(_info);
   }
}
