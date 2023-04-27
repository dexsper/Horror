using FishNet.Object;
using UnityEngine;

[RequireComponent(typeof(PlayerBehavior), typeof(PlayerCamera))]
public class PlayerInteraction : NetworkBehaviour
{
    [SerializeField] private LayerMask _interactableMask;
    [SerializeField] private float _interactionDistance = 1.5f;

    private PlayerBehavior _player;
    private PlayerCamera _playerCamera;

    public bool CanInteract { get; private set; }
    public IInteractable LookInteractable { get; private set; }


    private void Awake()
    {
        _player = GetComponent<PlayerBehavior>();
        _playerCamera = GetComponent<PlayerCamera>();
    }

    private void Update()
    {
        if (!base.IsServer && !base.IsOwner)
            return;

        if (LookInteractable != null)
        {
            CanInteract = LookInteractable.CanInteract(_player);
        }

        UpdateLookInteractable();
    }

    public void Interact()
    {
        if (LookInteractable != null && _player.IsOwner)
        {
            LookInteractable.Interact(_player);
        }
    }

    private void UpdateLookInteractable()
    {
        if (_playerCamera.Camera == null)
            return;

        RaycastHit hit;

        if (Physics.Raycast(_playerCamera.Camera.transform.position, _playerCamera.Camera.transform.forward, out hit, _interactionDistance, _interactableMask))
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
        if (!Application.isPlaying || _playerCamera.Camera == null)
            return;

        Gizmos.color = Color.green;
        Gizmos.DrawRay(_playerCamera.Camera.transform.position, _playerCamera.Camera.transform.forward * _interactionDistance);
    }
#endif
}
