using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class Bot : MonoBehaviour
{
    [SerializeField] private float _maxRandomWalk;

    //protected PlayerModel _playerModel;
    protected FieldOfView _fieldOfView;
    //protected SurvivorBehaviour _playerBehaviour;
    protected NavMeshAgent _agent;

    protected Vector3 _targetPosition;
    public bool IsMove { get; protected set; }

    protected virtual void Awake()
    {
        //_playerModel = GetComponent<PlayerModel>();
        _agent = GetComponent<NavMeshAgent>();
        _fieldOfView = GetComponent<FieldOfView>();
        //_playerBehaviour = GetComponent<SurvivorBehaviour>();
    }

    private void Update()
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

        //_playerModel.Animator.SetBool(nameof(IsMove), IsMove);
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