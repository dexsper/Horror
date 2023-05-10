using System;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public struct ManiacSettings
{
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

    [SerializeField] private Transform graphicalTransform;

    [Title("Current State")]
    [ShowInInspector, ReadOnly] public PlayerBehavior CurrentTarget { get; private set; }
    [field: SyncVar, ReadOnly] public bool IsMove { get; private set; }
    [field: SyncVar, ReadOnly] public bool IsAttack { get; private set; }

    public ManiacSettings Settings => _settings;
    public NavMeshAgent Agent { get; private set; }
    
    public ManiacAnimator ManiacAnimator { get; private set; }
    public ManiacApperaence ManiacApperaence { get; private set; }
    public FieldOfView View { get; private set; }
    public StateMachine StateMachine { get; private set; }
    public ManiacPatrolState PatrolState { get; private set; }
    public ManiacChaseState ChaseState { get; private set; }

    private float _targetLossTime = 0f;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        View = GetComponent<FieldOfView>();
        ManiacApperaence = GetComponent<ManiacApperaence>();
        ManiacAnimator = GetComponent<ManiacAnimator>();

        PatrolState = new ManiacPatrolState(this);
        ChaseState = new ManiacChaseState(this);
        StateMachine = new StateMachine(PatrolState);
    }

    private void Start()
    {
        var model = Instantiate(ManiacApperaence.GetManiacApperaence(), graphicalTransform.position, Quaternion.identity,
            graphicalTransform);
        ManiacAnimator.SetAnimator(model.GetComponent<Animator>());
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
}
