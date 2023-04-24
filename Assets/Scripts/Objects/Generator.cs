using System;
using System.Collections.Generic;
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
    public static List<Generator> Generators = new List<Generator>();
    
    [SerializeField] private GeneratorSettings _settings = default;

    [SerializeField] private ParticleSystem repairEffect;
    [SerializeField] private GameObject light;
    
    private float _repairTime = 0f;
    private IInteractable _thisInteractable;
  
    [SyncVar] private PlayerBehavior _repairInitiator;
    [SyncVar] private bool _isRepairing = false;
    [SyncVar(OnChange = nameof(On_RepairedChange))] private bool _isRepaired = false;

    public PlayerBehavior RepairInitiator => _repairInitiator;
    public bool IsRepairing => _isRepairing;
    public bool IsRepaired => _isRepaired;
    public string InteractionPrompt => _isRepairing ? "Stop Repair" : "Repair Generator";
    public Transform GetTransform() => this.transform;

    public static event Action<Generator> OnRepaired;
    
    private void Awake()
    {
        _thisInteractable = (IInteractable)this;
        Generators.Add(this);
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
                _isRepaired = true;
            }
        }
        else
        {
            _repairTime = 0f;
        }
    }

    private void On_RepairedChange(bool prev, bool next, bool asServer)
    {
        if (next)
        {
            OnRepaired?.Invoke(this);
            repairEffect.Play();
            light.SetActive(true);
        }
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
