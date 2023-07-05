using System;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerNickName;

    [SerializeField] private ParticleSystem emojiParticle;

    public void SetPlayerNickNameOnUI(Player player)
    {
        playerNickName.text = player.Data[LobbyManager.KEY_PLAYER_NAME].Value;
    }

    public void PlayEmojiParticle(Material emojiMaterial)
    {
        emojiParticle.gameObject.SetActive(false);
        ParticleSystemRenderer particleSystemRenderer = emojiParticle.GetComponent<ParticleSystemRenderer>();
        particleSystemRenderer.material = emojiMaterial;
        emojiParticle.GetComponent<ParticleSystemRenderer>().material = particleSystemRenderer.material;
        emojiParticle.gameObject.SetActive(true);
    }
}
