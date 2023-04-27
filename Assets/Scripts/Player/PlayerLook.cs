using FishNet.Object;
using UnityEngine;

[RequireComponent(typeof(PlayerBehavior), typeof(PlayerCamera))]
public class PlayerLook : NetworkBehaviour
{
    [SerializeField] private Vector2 cameraYLookRange;
    
    private PlayerBehavior _player;
    private PlayerCamera _playerCamera;

    private void Awake()
    {
        _player = GetComponent<PlayerBehavior>();
        _playerCamera = GetComponent<PlayerCamera>();
    }
    
    private void Update()
    {
        if (!base.IsOwner)
            return;

        Vector3 targetPosition = _playerCamera.CameraLook.transform.position;

        targetPosition.y += _player.Input.Look.y * Time.deltaTime;
        targetPosition.y = Mathf.Clamp(targetPosition.y, cameraYLookRange.x, cameraYLookRange.y);
        
        _playerCamera.CameraLook.transform.position = targetPosition;
    }
}
