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

   [field: SerializeField] public int maxPlayersCount { get; private set; } = 4;

   private void Awake()
   {
      joinRoomButton.onClick.AddListener(OnClick);
   }

   private RoomInfo _info;
   public void SetUp(RoomInfo roomInfo)
   {
      _info = roomInfo;
      roomPlayerCount.text = $"{_info.PlayerCount}/{maxPlayersCount}";
      roomNameText.text = _info.Name;
   }

   private void OnClick()
   {
      Launcher.Instance.JoinRoom(_info);
   }
}
