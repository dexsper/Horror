using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManiacAttackAnimationCanceler : MonoBehaviour
{
    private Animator _animator;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public bool IsAttack { get; private set; }

    public void CancelAttack()
    {
        _animator.SetBool("IsAttack",false);
    }
}
