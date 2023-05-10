using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ManiacAnimationEvents : MonoBehaviour
{
    [SerializeField] private List<AudioClip> stepSounds = new List<AudioClip>();

    private AudioSource _source;

    private void Awake()
    {
        _source = GetComponent<AudioSource>();
    }

    public void PlayStepSound()
    {
        _source.PlayOneShot(stepSounds[Random.Range(0,stepSounds.Count)]);
    }
}
