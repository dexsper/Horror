using FishNet.Object;
using UnityEngine;

public class PlayerInteraction : NetworkBehaviour
{
    [SerializeField] private LayerMask _interactableMask;
    [SerializeField] private float _interactionDistance = 1.5f;

    private IPlayerInput _playerInput;
    private Player _player;

    public bool CanInteract { get; private set; }
    public IInteractable LookInteractable { get; private set; }


    private void Awake()
    {
        _player = GetComponent<Player>();
        _playerInput = GetComponent<IPlayerInput>();
    }

    private void Update()
    {
        if (!base.IsOwner)
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
        Vector3 direction = _player.CameraLook.position - transform.position;

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
