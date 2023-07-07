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
    [SerializeField] private Sprite voiceSprite;
 
    private bool _isMuted = true;
    private Animator _animator;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _button = GetComponent<Button>();
        _animator = GetComponent<Animator>();
        
        _button.onClick.AddListener(MuteUnMute);
    }

    private void Update()
    {
        if (!_isMuted)
        {
            _image.sprite = voiceSprite;
        }
        
        UpdateAnimator(_isMuted);
    }

    private void MuteUnMute()
    {
        if (_isMuted)
        {
            UnMuteVoice();
        }
        else
        {
            MuteVoice();
        }
    }

    private void MuteVoice()
    {
        _isMuted = true;
        _image.sprite = unMutedSprite;
        VoiceChatController.Instance.SetMute(_isMuted);
    }

    private void UnMuteVoice()
    {
        _isMuted = false;
        _image.sprite = mutedSprite;
        VoiceChatController.Instance.SetMute(_isMuted);
    }

    private void UpdateAnimator(bool status)
    {
        if(_animator != null)
            _animator.SetBool("_isMuted",status);
    }
}
