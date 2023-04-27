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
        RaycastHit hit;
        
        Vector3 direction = _playerCamera.CameraLook.position - transform.position;

        if (Physics.Raycast(transform.position, direction, out hit, _interactionDistance, _interactableMask))
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
}
