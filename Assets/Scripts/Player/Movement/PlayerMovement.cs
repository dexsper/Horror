using System;
using FishNet;
using FishNet.Object;
using FishNet.Object.Prediction;
using FishNet.Transporting;
using UnityEngine;
using Zenject;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private float _moveRate = 15f;

    private IPlayerInput _playerInput;
    private Rigidbody _rigidbody;
    
    private void Awake()
    {
        _playerInput = GetComponent<IPlayerInput>();
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
        /* Server does not replay so it does
         * not need to add gravity. */
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
        /* Reconcile is sent during PostTick because we
         * want to send the rb data AFTER the simulation. */
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

        float horizontal = _playerInput.Movement.x;
        float vertical = _playerInput.Movement.y;

        if (horizontal == 0f && vertical == 0f)
            return;

        md = new MoveData(horizontal, vertical);
    }
    
    private void AddGravity()
    {
        _rigidbody.AddForce(Physics.gravity * 2f);
    }

    [Replicate]
    private void Move(MoveData md, bool asServer, Channel channel = Channel.Unreliable, bool replaying = false)
    {
        Vector3 forces = new Vector3(md.Horizontal, 0f, md.Vertical) * _moveRate;
        _rigidbody.AddForce(forces);
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