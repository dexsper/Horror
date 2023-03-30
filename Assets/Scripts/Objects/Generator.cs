using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

[System.Serializable]
public struct GeneratorSettings
{
    public float RepairTime;
}

public class Generator : NetworkBehaviour, IInteractable
{
    [SerializeField] private GeneratorSettings _settings = default;

    private float _repairTime = 0f;
    private IInteractable _thisInteractable;
    [SyncVar] private Player _repairInicitator;
    [SyncVar] private bool _isRepairing = false;
    [SyncVar] private bool _isRepaired = false;

    public Player RepairInicitator => _repairInicitator;
    public bool IsRepairing => _isRepairing;
    public string InteractionPrompt => _isRepairing ? "Stop Repair" : "Repair Generator";
    public Transform GetTransform() => this.transform;

    private void Awake()
    {
        _thisInteractable = (IInteractable)this;
    }

    [Server]
    private void Update()
    {
        if (_isRepairing && !_isRepaired)
        {
            if (RepairInicitator == null)
            {
                _isRepairing = false;
                return;
            }

            if (RepairInicitator.Interaction.LookInteractable != _thisInteractable)
            {
                _repairInicitator = null;
                _isRepairing = false;

                return;
            }

            _repairTime += Time.deltaTime;

            if (_repairTime >= _settings.RepairTime)
            {
                _isRepaired = true;
            }
        }
        else
        {
            _repairTime = 0f;
        }
    }


    [ServerRpc(RequireOwnership = false)]
    public void Interact(Player player)
    {
        _isRepairing = !_isRepairing;
        _repairInicitator = player;

        if (!_isRepairing)
            _repairInicitator = null;
    }

    public bool CanInteract(Player player)
    {
        if (_isRepairing && _repairInicitator != player)
            return false;

        return !_isRepaired;
    }
}
