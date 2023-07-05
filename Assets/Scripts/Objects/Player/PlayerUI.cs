using System;
using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Object;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class PlayerUI : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI playerNickName;

    [SerializeField] private ParticleSystem emojiParticle;
    [SerializeField] private List<Material> materials = new List<Material>();

    public void SetPlayerNickNameOnUI(Player player)
    {
        playerNickName.text = player.Data[LobbyManager.KEY_PLAYER_NAME].Value;
    }

    [ObserversRpc]
    public void PlayEmojiParticle(int emojiMaterialIndex,NetworkConnection conn)
    {
        emojiParticle.gameObject.SetActive(false);
        ParticleSystemRenderer particleSystemRenderer = emojiParticle.GetComponent<ParticleSystemRenderer>();
        particleSystemRenderer.material = materials[emojiMaterialIndex];
        emojiParticle.GetComponent<ParticleSystemRenderer>().material = particleSystemRenderer.material;
        emojiParticle.gameObject.SetActive(true);
    }
}
