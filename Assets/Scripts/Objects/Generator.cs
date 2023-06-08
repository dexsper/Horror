using System;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] private string _startRepairRu, _startRepairEn;
    [SerializeField] private string _stopRepairRu, _stopRepairEn;
    [SerializeField] private int neededRepairedGeneratorsCount;

    [Title("Effects")]
    [SerializeField] private ParticleSystem _repairedEffect;
    [SerializeField] private GameObject _lightEffect;
    [SerializeField] private AudioClip repairClip;

    private float _repairTime = 0f;
    private IInteractable _thisInteractable;
    private AudioSource _source;

    [SyncVar, HideInInspector] private bool _isRepairing = false;
    [SyncVar(OnChange = nameof(On_RepairedChange)), HideInInspector] private bool _isRepaired = false;
    [SyncVar, HideInInspector] private PlayerBehavior _repairInitiator;

    [SyncVar] public float RepairingProgress;

    [Title("Current State")]
    [ShowInInspector] public bool IsRepairing => _isRepairing;
    [ShowInInspector] public bool IsRepaired => _isRepaired;
    [ShowInInspector] public PlayerBehavior RepairInitiator => _repairInitiator;

    public string InteractionPrompt => _isRepairing ? GetStopRepairLocale() : GetStartRepairLocale();
    public Transform GetTransform() => this.transform;

    public static event Action<Generator> OnRepaired;


    private void Awake()
    {
        _source = GetComponent<AudioSource>();
        _thisInteractable = (IInteractable)this;

        Generators.Add(this);
    }

    private string GetStopRepairLocale()
    {
        return LocalizationUI.Instance.GetLocaleName() == "ru" ? _stopRepairRu : _stopRepairEn;
    }

    private string GetStartRepairLocale()
    {
        return LocalizationUI.Instance.GetLocaleName() == "ru" ? _startRepairRu : _startRepairEn;
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

            float repairingPercents = 1f / _settings.RepairTime;
            RepairingProgress = repairingPercents * _repairTime;
            _repairTime += Time.deltaTime;

            if (_repairTime >= _settings.RepairTime)
            {
                _isRepaired = true;
            }
        }
        else
        {
            _repairTime = 0f;
            RepairingProgress = 0f;
        }
    }

    public float GetRepairProgress()
    {
        return RepairingProgress;
    }

    private void On_RepairedChange(bool prev, bool next, bool asServer)
    {
        if (next)
        {
            _repairInitiator = null;
            _repairedEffect.Play();
            _lightEffect.SetActive(true);
            _source.PlayOneShot(repairClip);

            OnRepaired?.Invoke(this);
        }
    }

    public bool IsInteract(PlayerBehavior player)
    {
        return _isRepairing && _repairInitiator == player;
    }

    [ServerRpc(RequireOwnership = false)]
    public void Interact_RPC(PlayerBehavior player)
    {
        Interact(player);
    }

    [Server]
    public void Interact(PlayerBehavior player)
    {
        if (RepairInitiator != null && RepairInitiator != player)
            return;

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
