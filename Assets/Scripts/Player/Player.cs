using System;
using Cinemachine;
using FishNet.Component.Prediction;
using FishNet.Object;
using UnityEngine;
using Zenject;

public class Player : NetworkBehaviour
{
    [SerializeField] private Transform cameraLook;
    [SerializeField] private CinemachineVirtualCamera _cameraPrefab;

    private PredictedObject _predictedObject;
    
    public CinemachineVirtualCamera Camera { get; private set; }
    public Transform CameraLook => cameraLook; 

    private void Awake()
    {
        _predictedObject = GetComponent<PredictedObject>();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if(base.IsOwner)
        {
            FindObjectOfType<SceneContext>().Container.InjectGameObject(gameObject);
            CreateCamera();
        }
    }

    private void CreateCamera()
    {
        Camera = Instantiate(_cameraPrefab);
        Camera.Follow = _predictedObject.GetGraphicalObject();
        Camera.LookAt = cameraLook;
    }
}
