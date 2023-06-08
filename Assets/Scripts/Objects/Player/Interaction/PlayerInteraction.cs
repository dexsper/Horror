using FishNet.Object;
using UnityEngine;

[RequireComponent(typeof(PlayerBehavior), typeof(PlayerCamera))]
public class PlayerInteraction : NetworkBehaviour
{
    [SerializeField] private Transform _playerEyes;
    [SerializeField] private LayerMask _interactableMask;
    [SerializeField] private float _interactionDistance = 1.5f;

    private PlayerBehavior _player;
    private PlayerCamera _playerCamera;

    public bool CanInteract { get; private set; }
    public bool IsInteract { get; private set; }
    public IInteractable LookInteractable { get; private set; }


    private void Awake()
    {
        _player = GetComponent<PlayerBehavior>();
        _playerCamera = GetComponent<PlayerCamera>();
    }

    private void Update()
    {
        if (LookInteractable != null)
        {
            CanInteract = LookInteractable.CanInteract(_player);
            IsInteract = LookInteractable.IsInteract(_player);
        }

        UpdateLookInteractable();
    }

    public void Interact()
    {
        if (LookInteractable != null && _player.IsOwner)
        {
            LookInteractable.Interact_RPC(_player);
        }
    }

    private void UpdateLookInteractable()
    {
        if (_playerEyes == null)
            return;

        RaycastHit hit;
        Vector3 direction = _playerCamera.CameraLook.position - _playerEyes.position;

        if (Physics.Raycast(_playerEyes.position, direction, out hit, _interactionDistance, _interactableMask))
        {
            if (hit.collider.TryGetComponent(out IInteractable interactable))
            {
                if (LookInteractable == interactable || !interactable.CanInteract(_player))
                    return;

                LookInteractable = interactable;
                CanInteract = true;
                return;
            }
        }

        CanInteract = false;
        LookInteractable = null;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying || _playerEyes == null)
            return;

        Vector3 direction = _playerCamera.CameraLook.position - _playerEyes.position;

        Gizmos.color = Color.green;
        Gizmos.DrawRay(_playerEyes.position, direction);
    }
#endif
}
