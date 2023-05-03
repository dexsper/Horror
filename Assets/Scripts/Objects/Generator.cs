using System;
using System.Collections.Generic;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public struct GeneratorSettings
{
    public float RepairTime;
}

public class Generator : NetworkBehaviour, IInteractable
{
    public static List<Generator> Generators = new List<Generator>();

    [SerializeField] private GeneratorSettings _settings = default;

    [Title("Interaction")]
    [SerializeField] private string _startRepair = "Repair Generator";
    [SerializeField] private string _stopRepair = "Stop Repair";

    [Title("Effects")]
    [SerializeField] private ParticleSystem _repairedEffect;
    [SerializeField] private GameObject _lightEffect;

    private float _repairTime = 0f;
    private IInteractable _thisInteractable;

    [SyncVar, HideInInspector] private bool _isRepairing = false;
    [SyncVar(OnChange = nameof(On_RepairedChange)), HideInInspector] private bool _isRepaired = false;
    [SyncVar, HideInInspector] private PlayerBehavior _repairInitiator;

    [Title("Current State")]
    [ShowInInspector] public bool IsRepairing => _isRepairing;
    [ShowInInspector] public bool IsRepaired => _isRepaired;
    [ShowInInspector] public PlayerBehavior RepairInitiator => _repairInitiator;

    public string InteractionPrompt => _isRepairing ? _stopRepair : _startRepair;
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
        if (!IsServer)
            return;

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
            _repairInitiator = null;
            _repairedEffect.Play();
            _lightEffect.SetActive(true);

            OnRepaired?.Invoke(this);
        }
    }

    public bool IsInteract(PlayerBehavior player)
    {
        return _isRepairing && _repairInitiator == player;
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
