using Cinemachine;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

[RequireComponent(typeof(PlayerBehavior))]
public class PlayerCamera : NetworkBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _cameraPrefab;
    [SerializeField] private Transform _cameraLook;

    private PlayerBehavior _player;

    public CinemachineVirtualCamera Camera { get; private set; }
    public Transform CameraLook => _cameraLook;

    private void Awake()
    {
        _player = GetComponent<PlayerBehavior>();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (base.IsOwner)
        {
            CreateCamera();
        }
    }

    private void CreateCamera()
    {
        Camera = Instantiate(_cameraPrefab);

        Camera.Follow = _player.PredictedObject.GetGraphicalObject();
        Camera.LookAt = _cameraLook;
    }

    [TargetRpc]
    public void SetActive(NetworkConnection conn, bool active)
    {   
        Camera.gameObject.SetActive(active);
    }
}
