using System;
using FishNet;
using FishNet.Object;
using FishNet.Object.Prediction;
using FishNet.Transporting;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private float _moveRate = 15f;
    
    private IPlayerInput _playerInput;

    private Rigidbody _rigidbody;

    private void Awake()
    {
        _playerInput = GetComponent<IPlayerInput>();
        _rigidbody = GetComponent<Rigidbody>();
        
        InstanceFinder.TimeManager.OnTick += TimeManagerOnOnTick;
        InstanceFinder.TimeManager.OnPostTick += TimeManagerOnOnPostTick;
    }

    private void TimeManagerOnOnTick()
    {
        
        if (base.IsOwner)
        {
            Reconciliation(default,false);
            BuildMoveData(out MoveData md);
            Move(md, false);
        }

        if (base.IsServer)
        {
            Move(default, true);
        }

        AddGravity();
    }
    
    private void TimeManagerOnOnPostTick()
    {
        if (base.IsServer)
        {
            ReconcileData rd = new ReconcileData(transform.position, transform.rotation, _rigidbody.velocity, _rigidbody.angularVelocity);
            Reconciliation(rd, true);
        }
    }
    
    private void BuildMoveData(out MoveData md)
    {
        md = default;

        Vector2 input = _playerInput.Movement;

        if (input == Vector2.zero)
            return;

        md = new MoveData(input.x, input.y);
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
        if (transform.position != rd.Position)
        {
            Debug.Log($"** LocalTick: {TimeManager.LocalTick}, Mispredicted Position: {transform.position.ToString("F6")}, {rd.Position.ToString("F6")}");
        }
        
        transform.position = rd.Position;
        transform.rotation = rd.Rotation;
        _rigidbody.velocity = rd.Velocity;
        _rigidbody.angularVelocity = rd.AngularVelocity;
    }
}
