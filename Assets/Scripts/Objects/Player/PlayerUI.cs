using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerNickName;

    public void SetPlayerNickNameOnUI(Player player)
    {
        playerNickName.text = player.Data[LobbyManager.KEY_PLAYER_NAME].Value;
    }
}
