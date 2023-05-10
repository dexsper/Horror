using System;
using System.Collections.Generic;
using FishNet.Component.Prediction;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[System.Serializable]
public struct ManiacSettings
{
    [Title("Models")] public List<GameObject> ModelsList;
    
    [Title("Patrol")]
    [Range(0.5f, 15f)] public float PatrolSleepTime;

    [Title("Attack")]
    [Range(1f, 5f)] public float AttackDistance;
    [Range(0.5f, 5f)] public float AttackDelayTime;
    [Range(1, 100f)] public float AttackDamage;
    [Range(0.2f, 3f)] public float ResetTargetTime;
}

[RequireComponent(typeof(NavMeshAgent), typeof(FieldOfView))]
public class ManiacBehaviour : NetworkBehaviour
{
    [SerializeField] private ManiacSettings _settings;

    [Title("Current State")]
    [ShowInInspector, ReadOnly] public PlayerBehavior CurrentTarget { get; private set; }
    [ShowInInspector,ReadOnly][field: SyncVar, HideInInspector] public bool IsMove { get; private set; }
    [ShowInInspector,ReadOnly][field: SyncVar, HideInInspector] public bool IsAttack { get; private set; }

    public ManiacSettings Settings => _settings;
    public NavMeshAgent Agent { get; private set; }

    public Animator Animator { get; private set; }

    public PredictedObject PredictedObject { get; private set; }
    public FieldOfView View { get; private set; }
    public StateMachine StateMachine { get; private set; }
    public ManiacPatrolState PatrolState { get; private set; }
    public ManiacChaseState ChaseState { get; private set; }

    private float _targetLossTime = 0f;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        View = GetComponent<FieldOfView>();
        PredictedObject = GetComponent<PredictedObject>();

        PatrolState = new ManiacPatrolState(this);
        ChaseState = new ManiacChaseState(this);
        StateMachine = new StateMachine(PatrolState);
    }

    [Server]
    public override void OnStartServer()
    {
        base.OnStartServer();
        int index = Random.Range(0, _settings.ModelsList.Count);
        CreateModelRPC(index);
    }

    [Server]
    private void Update()
    {
        if (!IsServer)
            return;

        if (StateMachine != null && StateMachine.CurrentState != null)
        {
            StateMachine.CurrentState.Update();
        }

        if (CurrentTarget != null)
        {
            if (!View.IsInSight(CurrentTarget.gameObject))
            {
                _targetLossTime += Time.deltaTime;

                if (_targetLossTime >= Settings.ResetTargetTime)
                    CurrentTarget = null;
            }
            else
            {
                _targetLossTime = 0f;
            }
        }

        if (CurrentTarget == null)
        {
            _targetLossTime = 0f;

            CurrentTarget = View.GetObject<PlayerBehavior>();
        }

        IsMove = Agent.velocity.sqrMagnitude > 0f;
        IsAttack = ChaseState.IsAttack;
    }

    [ObserversRpc(BufferLast = true, RunLocally = true)]
    private void CreateModelRPC(int modelIndex)
    {
        var model = Instantiate(_settings.ModelsList[modelIndex], PredictedObject.GetGraphicalObject());
        Animator = model.GetComponent<Animator>();
    }
}
