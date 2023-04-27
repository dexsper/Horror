using FishNet;
using FishNet.Object;
using FishNet.Object.Prediction;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
using UnityEngine;

[RequireComponent(typeof(PlayerBehavior), typeof(Rigidbody))]
public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private float _speed = 2f;
    [SerializeField] private float _gravityMultiplier = 4f;

    private float _xRototation;
    private bool _lockCursor;

    private PlayerBehavior _player;
    private Rigidbody _rigidbody;

    [field: SyncVar]
    public bool IsMove { get; [ServerRpc(RunLocally = true)] private set; }

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
        base.PredictionManager.OnPreReplicateReplay += PredictionManager_OnPreReplicateReplay;
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        base.PredictionManager.OnPreReplicateReplay -= PredictionManager_OnPreReplicateReplay;
    }

    private void PredictionManager_OnPreReplicateReplay(uint arg1, PhysicsScene arg2, PhysicsScene2D arg3)
    {
        if (!base.IsServer)
            AddGravity();
    }


    private void TimeManager_OnTick()
    {
        if (base.IsOwner)
        {
            Reconciliation(default, false);
            BuildMoveData(out MoveData md);
            Move(md, false);
        }

        if (base.IsServer)
        {
            Move(default, true);
        }

        AddGravity();
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
        _rigidbody.AddForce(Physics.gravity * _gravityMultiplier);
    }

    [Replicate]
    private void Move(MoveData md, bool asServer, Channel channel = Channel.Unreliable, bool replaying = false)
    {
        float delta = (float)base.TimeManager.TickDelta;

        Vector3 targetRotation = new Vector3(0, transform.localEulerAngles.y + md.Rotation * delta);
        transform.localEulerAngles = targetRotation;

        Vector3 velocity = Quaternion.LookRotation(_player.transform.forward) * new Vector3(md.Horizontal, 0f, md.Vertical);

        _rigidbody.velocity = velocity * _speed;

        IsMove = _rigidbody.velocity.sqrMagnitude > 0f;
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