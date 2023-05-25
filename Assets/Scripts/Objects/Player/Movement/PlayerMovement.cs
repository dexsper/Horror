using FishNet;
using FishNet.Object;
using FishNet.Object.Prediction;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(PlayerBehavior), typeof(Rigidbody))]
public class PlayerMovement : NetworkBehaviour
{
    [Title("Climb")]
    [SerializeField] private Transform _stepUpper;
    [SerializeField] private Transform _stepLower;
    [SerializeField] private float _stepSmooth = 0.1f;
    [SerializeField] private float _gravityMultiplier = 4f;

    [Title("Movement")]
    [SerializeField] private float _speed = 2f;

    private float _xRototation;
    private bool _lockCursor;

    private PlayerBehavior _player;
    private Rigidbody _rigidbody;

    [Title("Current State")]
    [ShowInInspector, ReadOnly]
    [field: SyncVar, HideInInspector]
    public bool IsMove { get; [ServerRpc(RunLocally = true)] private set; }

    [ShowInInspector, ReadOnly]
    [field: SyncVar, HideInInspector]
    public bool IsStep { get; [ServerRpc(RunLocally = true)] private set; }

    private void Awake()
    {
        _player = GetComponent<PlayerBehavior>();
        _rigidbody = GetComponent<Rigidbody>();

        InstanceFinder.TimeManager.OnTick += TimeManager_OnTick;
        InstanceFinder.TimeManager.OnPostTick += TimeManager_OnPostTick;
    }
    private void OnDestroy()
    {
        if (InstanceFinder.TimeManager != null)
        {
            InstanceFinder.TimeManager.OnTick -= TimeManager_OnTick;
            InstanceFinder.TimeManager.OnPostTick -= TimeManager_OnPostTick;
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
    }
    public override void OnStopClient()
    {
        base.OnStopClient();
    }

    private void TimeManager_OnTick()
    {
        if (base.IsOwner)
        {
            Reconciliation(default, false);
            BuildMoveData(out MoveData md);
            Move(md, false);

            UpdateStates();
        }

        if (base.IsServer)
        {
            Move(default, true);
            AddGravity();
        }
    }
    private void TimeManager_OnPostTick()
    {
        if (base.IsServer)
        {
            ReconcileData rd = new ReconcileData(transform.position, transform.rotation, _rigidbody.velocity,
                _rigidbody.angularVelocity);

            Reconciliation(rd, true);
        }
    }

    private void BuildMoveData(out MoveData md)
    {
        md = default;

        var movement = _player.Input.Movement;

        if (movement == Vector2.zero && _player.Input.Look.x == 0f)
            return;

        md = new MoveData(movement.x, movement.y, _player.Input.Look.x);
    }
    private void AddGravity()
    {
        IsStep = false;

        if (IsMove)
        {
            if (Physics.Raycast(_stepLower.position, transform.TransformDirection(Vector3.forward), out RaycastHit lowerHit, 0.4f))
            {
                if (!Physics.Raycast(_stepUpper.position, transform.TransformDirection(Vector3.forward), out RaycastHit upperHit, 0.4f))
                {
                    IsStep = true;
                }
            }
        }

        if (!IsStep)
            _rigidbody.AddForce(Physics.gravity * _gravityMultiplier);
    }
    private void UpdateStates()
    {
        IsMove = _rigidbody.velocity.sqrMagnitude >= 0.1f;
    }

    [Replicate]
    private void Move(MoveData md, bool asServer, Channel channel = Channel.Unreliable, bool replaying = false)
    {
        float delta = (float)base.TimeManager.TickDelta;

        Vector3 targetRotation = new Vector3(0, transform.localEulerAngles.y + md.Rotation * delta);
        transform.localEulerAngles = targetRotation;

        Vector3 velocity = Quaternion.LookRotation(_player.transform.forward) * new Vector3(md.Horizontal, 0f, md.Vertical);
        velocity.y = IsStep ? _stepSmooth : 0f;

        _rigidbody.velocity = velocity * _speed;
    }

    [Reconcile]
    private void Reconciliation(ReconcileData rd, bool asServer, Channel channel = Channel.Unreliable)
    {
        transform.position = rd.Position;
        transform.rotation = rd.Rotation;

        _rigidbody.velocity = rd.Velocity;
        _rigidbody.angularVelocity = rd.AngularVelocity;
    }
}