using FishNet.Object;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class Bot : NetworkBehaviour
{
    [SerializeField] private float _maxRandomWalk;
    
    protected FieldOfView _fieldOfView;
    protected NavMeshAgent _agent;

    protected Vector3 _targetPosition;
    public bool IsMove { get; protected set; }

    protected virtual void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _fieldOfView = GetComponent<FieldOfView>();
    }

    [Server]
    protected virtual void Update()
    {
        Move();
        UpdateState();
    }

    protected virtual void UpdateState()
    {
        if (_agent.remainingDistance <= _agent.stoppingDistance)
        {
            IsMove = false;
            _targetPosition = Vector3.zero;
        }
    }

    protected virtual void Move()
    {
        var target = GetTargetPosition();

        if(target != Vector3.zero)
            _targetPosition = target;

        if (_targetPosition == Vector3.zero)
            _targetPosition = GetRandomPosition();

        if (_targetPosition != Vector3.zero)
        {
            _agent.SetDestination(_targetPosition);

            IsMove = true;
        }
    }

    protected virtual Vector3 GetTargetPosition()
    {
        return Vector3.zero;
    }

    protected Vector3 GetRandomPosition()
    {
        Vector3 position = Vector3.zero;

        Vector3 direction = Random.insideUnitSphere * Random.Range(_maxRandomWalk / 2f, _maxRandomWalk);
        direction += transform.position;

        NavMeshHit hit;
        bool foundPosition = NavMesh.SamplePosition(direction, out hit, _maxRandomWalk, NavMesh.AllAreas);

        if (foundPosition)
        {
            NavMeshPath path = new NavMeshPath();
            _agent.CalculatePath(hit.position, path);

            if (path.status == NavMeshPathStatus.PathComplete)
                position = hit.position;
        }

        return position;
    }
}