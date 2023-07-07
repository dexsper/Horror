using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class ManiacSound : NetworkBehaviour
{
    private float _timerCounter, _timer = 10f;

    [SerializeField] private AudioSource source;
    
    private void Update()
    {
        _timerCounter += Time.deltaTime;
        if (_timerCounter >= _timer)
        {
            _timerCounter = _timer;
            PlaySound();
        }
    }

    [Server]
    private void PlaySound()
    {
        _timerCounter = 0f;
        source.Play();
    }
}
