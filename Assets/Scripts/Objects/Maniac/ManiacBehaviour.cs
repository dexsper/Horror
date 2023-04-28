using FishNet.Object;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public struct ManiacSettings
{
    [Title("Patrol")]
    [Range(0.5f, 15f)] public float PatrolSleepTime;
}

public class ManiacBehaviour : NetworkBehaviour
{
    [SerializeField] private ManiacSettings _settings;

    public ManiacSettings Settings => _settings;
    public NavMeshAgent Agent { get; private set; }

    public StateMachine StateMachine { get; private set; }
    public ManiacPatrolState PatrolState { get; private set; }

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();

        PatrolState = new ManiacPatrolState(this);
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
    }
}
