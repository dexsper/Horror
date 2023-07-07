using System;
using System.Collections.Generic;
using DG.Tweening;
using FishNet.Connection;
using FishNet.Object;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PlayerUI : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI playerNickName;

    [SerializeField] private ParticleSystem emojiParticle;
    [SerializeField] private List<Material> materials = new List<Material>();

    [SerializeField] private Image damageImage;
    [SerializeField] private List<Sprite> bloodImagesList = new List<Sprite>();
    
    public void SetPlayerNickNameOnUI(Player player)
    {
        playerNickName.text = player.Data[LobbyManager.KEY_PLAYER_NAME].Value;
    }
    
    public void PlayDamageImage()
    {
        var index = Random.Range(0, bloodImagesList.Count);
        damageImage.sprite = bloodImagesList[index];
        
        damageImage.DOColor(new Color(1f, 1f, 1f, 0f), 0.25f).From(Color.white).SetEase(Ease.Linear);
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
