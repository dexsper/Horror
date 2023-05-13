using UnityEngine;

public class ManiacAnimator : MonoBehaviour
{
    private ManiacBehaviour _maniacBehaviour;

    public bool IsMove => _maniacBehaviour.IsMove;
    public bool IsAttack => _maniacBehaviour.IsAttack;

    private void Awake()
    {
        _maniacBehaviour = GetComponent<ManiacBehaviour>();
    }

    private void Update()
    {
        if (_maniacBehaviour.Animator != null)
        {
            _maniacBehaviour.Animator.SetBool(nameof(IsMove),IsMove);
            _maniacBehaviour.Animator.SetBool(nameof(IsAttack), IsAttack);
        }
    }
}
