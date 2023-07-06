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

    [ServerRpc]
    public void CreateEmoji(int index, NetworkConnection conn = null)
    {
        
        PlayEmojiParticle(index,conn);
    }
    
    [ObserversRpc]
    private void PlayEmojiParticle(int emojiMaterialIndex, NetworkConnection conn)
    {
        if(this.Owner != conn)
            return;
        ParticleSystemRenderer particleSystemRenderer = emojiParticle.GetComponent<ParticleSystemRenderer>();
        particleSystemRenderer.material = materials[emojiMaterialIndex];
        emojiParticle.GetComponent<ParticleSystemRenderer>().material = particleSystemRenderer.material;
        emojiParticle.gameObject.SetActive(true);
        emojiParticle.Play();
    }
}
