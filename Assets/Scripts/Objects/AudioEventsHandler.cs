using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioEventsHandler : MonoBehaviour
{
    [SerializeField] private List<AudioClip> stepSounds = new List<AudioClip>();

    [SerializeField] private AudioClip repairClip;
    
    private AudioSource _source;

    private void Awake()
    {
        _source = GetComponent<AudioSource>();
    }

    public void PlayStepSound()
    {
        _source.PlayOneShot(stepSounds[Random.Range(0,stepSounds.Count)]);
    }

    public void PlayRepairSound()
    {
        _source.PlayOneShot(repairClip);
    }
}
