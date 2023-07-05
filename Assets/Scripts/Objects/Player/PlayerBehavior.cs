using System;
using System.Collections.Generic;
using FishNet.Component.Prediction;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(PredictedObject), typeof(PlayerInteraction))]
[RequireComponent(typeof(Health))]
public class PlayerBehavior : NetworkBehaviour
{

    public PredictedObject PredictedObject { get; private set; }
    public Health Health { get; private set; }
    public PlayerInteraction Interaction { get; private set; }
    public PlayerMovement Movement { get; private set; }
    public IPlayerInput Input { get; private set; }
    public Animator Animator { get; private set; }
    public PlayerCamera PlayerCamera { get; private set; }
    public PlayerUI PlayerUI { get; private set; }

    public GameObject Model { get; private set; }

    [Title("Current State")]
    [ShowInInspector, ReadOnly]
    [field: SyncVar, HideInInspector]
    public bool IsLeaved { get; private set; }

    public static PlayerBehavior LocalPlayer { get; private set; }
    public static List<PlayerBehavior> Players { get; private set; } = new List<PlayerBehavior>();

    public static event Action<PlayerBehavior> OnDead;
    public static event Action<PlayerBehavior> OnLeaved;
    public static event Action<PlayerBehavior> OnRespawned;
    public static event Action OnPlayerRemoved;

    private void Awake()
    {
        PredictedObject = GetComponent<PredictedObject>();
        Health = GetComponent<Health>();
        Interaction = GetComponent<PlayerInteraction>();
        Movement = GetComponent<PlayerMovement>();
        Input = GetComponent<IPlayerInput>();
        PlayerUI = GetComponent<PlayerUI>();
        PlayerCamera = GetComponent<PlayerCamera>();

        Health.OnDead += () => OnDead?.Invoke(this);
        Health.OnRestored += () => OnRespawned?.Invoke(this);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (base.IsOwner)
        {
            FindObjectOfType<SceneContext>().Container.InjectGameObject(gameObject);

            LocalPlayer = this;
        }

        if (!Players.Contains(this))
        {
            Players.Add(this);
        }

        OnRespawned?.Invoke(this);
    }
    public override void OnStopClient()
    {
        base.OnStopClient();

        if (base.IsOwner) LocalPlayer = null;

        Players.Remove(this);
        OnPlayerRemoved?.Invoke();
    }

    private void Update()
    {
        UpdateAnimations();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (IsLeaved)
            return;

        if (other.TryGetComponent(out LeaveZone leaveZone))
        {
            IsLeaved = true;
            OnLeaved?.Invoke(this);
        }
    }

    private void UpdateAnimations()
    {
        if (Animator == null)
            return;

        if (Movement != null)
            Animator.SetBool(nameof(Movement.IsMove), Movement.IsMove);

        if (Interaction != null)
            Animator.SetBool(nameof(Interaction.IsInteract), Interaction.IsInteract);
    }

    public void UpdateModel(GameObject model)
    {
        Debug.Log(model);

        Model = Instantiate(model, PredictedObject.GetGraphicalObject());

        foreach (var render in Model.GetComponentsInChildren<Renderer>())
        {
            render.enabled = !base.IsOwner;
        }

        Animator = Model.GetComponent<Animator>();
    }
}