using System.Collections.Generic;
using Cinemachine;
using FishNet.Component.Prediction;
using FishNet.Object;
using UnityEngine;
using Zenject;

public class PlayerBehavior : NetworkBehaviour
{
    [SerializeField] private Transform cameraLook;
    [SerializeField] private CinemachineVirtualCamera _cameraPrefab;

    private PredictedObject _predictedObject;
    private PlayerInteraction _interaction;
    private PlayerMovement _movement;

    public PredictedObject PredictedObject => _predictedObject;
    public PlayerInteraction Interaction => _interaction;
    public PlayerMovement Movement => _movement;
    public CinemachineVirtualCamera Camera { get; private set; }
    public Transform CameraLook => cameraLook;

    public static PlayerBehavior LocalPlayer { get; private set; }
    public static List<PlayerBehavior> Players { get; private set; } = new List<PlayerBehavior>();


    private void Awake()
    {
        _predictedObject = GetComponent<PredictedObject>();
        _interaction = GetComponent<PlayerInteraction>();
        _movement = GetComponent<PlayerMovement>();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (base.IsOwner)
        {
            FindObjectOfType<SceneContext>().Container.InjectGameObject(gameObject);
            CreateCamera();

            LocalPlayer = this;
        }

        if (!Players.Contains(this))
        {
            Players.Add(this);
        }
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        
        if (base.IsOwner) LocalPlayer = null;

        Players.Remove(this);
    }

    private void CreateCamera()
    {
        Camera = Instantiate(_cameraPrefab);
        Camera.Follow = _predictedObject.GetGraphicalObject();
        Camera.LookAt = cameraLook;
    }
}
