using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManiacAnimator : MonoBehaviour
{
    private Animator _animator;

    private ManiacBehaviour _maniacBehaviour;

    public bool IsMove => _maniacBehaviour.IsMove;
    public bool IsAttack => _maniacBehaviour.IsAttack;

    private void Awake()
    {
        _maniacBehaviour = GetComponent<ManiacBehaviour>();
    }

    public void SetAnimator(Animator animator)
    {
        _animator = animator;
    }

    private void Update()
    {
        if (_animator != null)
        {
            _animator.SetBool(nameof(IsMove),IsMove);
            _animator.SetBool(nameof(IsAttack),IsAttack);
        }
    }
}
