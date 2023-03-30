using System;
using Cinemachine;
using FishNet.Object;
using UnityEngine;

public class PlayerLook : NetworkBehaviour
{
    [SerializeField] private Vector2 cameraYLookRange;
    
    private Player _player;
    private IPlayerInput _playerInput;

    private void Awake()
    {
        _player = GetComponent<Player>();
        _playerInput = GetComponent<IPlayerInput>();
    }
    
    private void Update()
    {
        if (!base.IsOwner)
            return;

        Vector3 targetPosition = _player.CameraLook.transform.position;

        targetPosition.y += _playerInput.Look.y * Time.deltaTime;
        targetPosition.y = Mathf.Clamp(targetPosition.y, cameraYLookRange.x, cameraYLookRange.y);
        
        _player.CameraLook.transform.position = targetPosition;
    }
}
