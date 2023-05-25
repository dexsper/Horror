using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VoiceMute : MonoBehaviour
{
    private Image _image;
    private Button _button;
    
    [SerializeField] private Sprite mutedSprite, unMutedSprite;
 
    private bool _isMuted;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _button = GetComponent<Button>();
        
        _button.onClick.AddListener(MuteUnMute);
    }

    private void MuteUnMute()
    {
        if (_isMuted)
        {
            _isMuted = false;
            _image.sprite = mutedSprite;
            VoiceChatController.Instance.SetMute(_isMuted);
        }
        else
        {
            _isMuted = true;
            _image.sprite = unMutedSprite;
            VoiceChatController.Instance.SetMute(_isMuted);
        }
    }
}
