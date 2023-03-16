using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerAnimator : MonoBehaviour
{
    private Animator _playerAnimator;
    private AudioSource _source; 
    
    [SerializeField] private List<AudioClip> stepClips = new List<AudioClip>();
    [SerializeField] private string walkAnimationName,runAnimationName;

    private void Start()
    {
        _playerAnimator = GetComponent<Animator>();
        _source = GetComponent<AudioSource>();
    }

    public void SetBlendParameter(float left, float right, float vertical, float backward)
    {
        _playerAnimator.SetFloat("left",left);
        _playerAnimator.SetFloat("right",right);
        _playerAnimator.SetFloat("vertical",vertical);
        _playerAnimator.SetFloat("backward",backward);
    }

    public void StartWalkAnimation()
    {
        _playerAnimator.SetBool(walkAnimationName,true);
    }

    public void CancelWalkAnimation()
    {
        _playerAnimator.SetBool(walkAnimationName,false);
    }

    public void StartRunAnimation()
    {
        _playerAnimator.SetBool(runAnimationName,true);
    }

    public void CancelRunAnimation()
    {
        _playerAnimator.SetBool(runAnimationName,false);
    }
    
    private AudioClip PickRandomStepClip()
    {
        return stepClips[Random.Range(0, stepClips.Count)];
    }
    
    public void PlayStepSound()
    {
        _source.PlayOneShot(PickRandomStepClip());
    }
}
