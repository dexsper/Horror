using FishNet;
using FishNet.Object;
using FishNet.Object.Prediction;
using FishNet.Transporting;
using UnityEngine;


public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private float _speed = 2f;
    [SerializeField] private float _gravityMultiplier = 4f;
    [SerializeField] private float sensitivity;
    
    private float _xRototation;
    private bool _lockCursor;
    private Transform _playerBody;

    private IPlayerInput _playerInput;
    private Rigidbody _rigidbody;
    private Player _player;

    private void Awake()
    {
        _playerInput = GetComponent<IPlayerInput>();
        _rigidbody = GetComponent<Rigidbody>();
        _playerBody = _rigidbody.gameObject.transform;
        _player = GetComponent<Player>();

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
            BuildLookData(out LookData ld);
            Move(md, false);
            LookRotation(ld);
        }

        if (base.IsServer)
        {
            Move(default, true);
            LookRotation(default);
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

    private void LookRotation(LookData ld)
    {
        float mouX = ld.Horizontal * sensitivity * Time.deltaTime;
        float mouY = ld.Vertical * sensitivity * Time.deltaTime;

        _xRototation -= mouY;
        _xRototation = Mathf.Clamp(_xRototation, -90f, 35f);

        
        _player.PlayerCameraTransform.localRotation = Quaternion.Euler(_xRototation, 0f, 0f);

        _playerBody.Rotate(Vector3.up * mouX);
    }

    private void BuildLookData(out LookData ld)
    {
        ld = default;
        var look = _playerInput.LookDirection;
        
        if(look == Vector2.zero)
            return;
        ld = new LookData(look.x,look.y);
    }

private void BuildMoveData(out MoveData md)
    {
        md = default;

        var movement = _playerInput.Movement;

        if (movement == Vector2.zero)
            return;

        md = new MoveData(movement.x, movement.y);
    }

    private void AddGravity()
    {
        _rigidbody.AddForce(Physics.gravity * _gravityMultiplier);
    }

    [Replicate]
    private void Move(MoveData md,bool asServer, Channel channel = Channel.Unreliable, bool replaying = false)
    {
        Vector3 velocity = new Vector3(md.Horizontal, 0f, md.Vertical) * _speed;
        
        _rigidbody.velocity = velocity;
        
       
    }

    
    /*private void Look(LookData ld, bool asServer, Channel channel = Channel.Unreliable, bool replaying = false)
    {
        
        float mouX = ld.Horizontal * sensitivity * Time.deltaTime;
        float mouY = ld.Vertical * sensitivity * Time.deltaTime;

        _xRototation -= mouY;
        _xRototation = Mathf.Clamp(_xRototation, -90f, 35f);

        transform.localRotation = Quaternion.Euler(_xRototation, 0f, 0f);

        _playerBody.Rotate(Vector3.up * mouX);
    }*/

    [Reconcile]
    private void Reconciliation(ReconcileData rd, bool asServer, Channel channel = Channel.Unreliable)
    {
        transform.position = rd.Position;
        transform.rotation = rd.Rotation;

        _rigidbody.velocity = rd.Velocity;
        _rigidbody.angularVelocity = rd.AngularVelocity;
    }
}