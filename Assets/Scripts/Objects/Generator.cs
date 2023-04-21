using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using Zenject;

[System.Serializable]
public struct GeneratorSettings
{
    public float RepairTime;
}

public class Generator : NetworkBehaviour, IInteractable
{
    [SerializeField] private GeneratorSettings _settings = default;

    [SerializeField] private ParticleSystem repairEffect;
    [SerializeField] private GameObject light;
    
    private float _repairTime = 0f;
    private IInteractable _thisInteractable;
    [Inject] private ObjectsController _objectsController;
    [SyncVar] private PlayerBehavior _repairInitiator;
    [SyncVar] private bool _isRepairing = false;
    [SyncVar] private bool _isRepaired = false;

    public PlayerBehavior RepairInitiator => _repairInitiator;
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
            if (RepairInitiator == null)
            {
                _isRepairing = false;
                return;
            }

            if (RepairInitiator.Interaction.LookInteractable != _thisInteractable)
            {
                _repairInitiator = null;
                _isRepairing = false;

                return;
            }

            _repairTime += Time.deltaTime;

            if (_repairTime >= _settings.RepairTime)
            {
                RepairGenerator();
            }
        }
        else
        {
            _repairTime = 0f;
        }
    }

    [ObserversRpc(BufferLast = true)]
    private void RepairGenerator()
    {
        _isRepaired = true;
        _objectsController.OnGeneratorRepaired(this);
        repairEffect.Play();
        light.SetActive(true);
    }

    [ServerRpc(RequireOwnership = false)]
    public void Interact(PlayerBehavior player)
    {
        _isRepairing = !_isRepairing;
        _repairInitiator = player;

        if (!_isRepairing)
            _repairInitiator = null;
    }

    public bool CanInteract(PlayerBehavior player)
    {
        if (_isRepairing && _repairInitiator != player)
            return false;

        return !_isRepaired;
    }
}
