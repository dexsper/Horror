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
        _image.sprite = !_isMuted ? unMutedSprite : mutedSprite;

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
        VoiceChatController.Instance.SetMute(_isMuted);
    }

    private void UnMuteVoice()
    {
        _isMuted = false;
        VoiceChatController.Instance.SetMute(_isMuted);
    }

    private void UpdateAnimator(bool status)
    {
        if(_animator != null)
            _animator.SetBool("_isMuted",status);
    }
}
