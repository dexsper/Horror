using FishNet.Object;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public struct ManiacSettings
{
    [Title("Patrol")]
    [Range(0.5f, 15f)] public float PatrolSleepTime;

    [Title("Attack")]
    [Range(0.2f, 3f)] public float ResetTargetTime;
}

[RequireComponent(typeof(NavMeshAgent), typeof(FieldOfView))]
public class ManiacBehaviour : NetworkBehaviour
{
    [SerializeField] private ManiacSettings _settings;


    [Title("Current State")]
    [ShowInInspector, ReadOnly] public PlayerBehavior CurrentTarget { get; private set; }

    public ManiacSettings Settings => _settings;

    public NavMeshAgent Agent { get; private set; }
    public FieldOfView View { get; private set; }
    public StateMachine StateMachine { get; private set; }
    public ManiacPatrolState PatrolState { get; private set; }
    public ManiacChaseState ChaseState { get; private set; }

    private float _targetLossTime = 0f;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        View = GetComponent<FieldOfView>();

        PatrolState = new ManiacPatrolState(this);
        ChaseState = new ManiacChaseState(this);
        StateMachine = new StateMachine(PatrolState);
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
    }
}
