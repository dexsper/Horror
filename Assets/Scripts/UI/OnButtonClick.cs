using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource),typeof(Button))]
public class OnButtonClick : MonoBehaviour
{
    private AudioSource _source;
    private Button _button;

    [SerializeField] private AudioClip clickSound;
    
    private void Awake()
    {
        _source = GetComponent<AudioSource>();
        _button = GetComponent<Button>();

        _source.playOnAwake = false;
        
        _button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        _source.PlayOneShot(clickSound);
    }
}
