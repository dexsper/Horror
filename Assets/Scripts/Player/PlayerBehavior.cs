using System.Collections.Generic;
using FishNet.Component.Prediction;
using FishNet.Object;
using UnityEngine;
using Zenject;

public class PlayerBehavior : NetworkBehaviour
{
    private Animator _playerAnimator;

    public PredictedObject PredictedObject { get; private set; }
    public PlayerInteraction Interaction { get; private set; }
    public PlayerMovement Movement { get; private set; }
    public IPlayerInput Input { get; private set; }

    public GameObject Model { get; private set; }

    public static PlayerBehavior LocalPlayer { get; private set; }
    public static List<PlayerBehavior> Players { get; private set; } = new List<PlayerBehavior>();


    private void Awake()
    {
        PredictedObject = GetComponent<PredictedObject>();
        Interaction = GetComponent<PlayerInteraction>();
        Movement = GetComponent<PlayerMovement>();
        Input = GetComponent<IPlayerInput>();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (base.IsOwner)
        {
            FindObjectOfType<SceneContext>().Container.InjectGameObject(gameObject);

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

    private void Update()
    {
        UpdateAnimations();
    }

    private void UpdateAnimations()
    {
        if (_playerAnimator == null)
            return;
        
        _playerAnimator.SetBool(nameof(Movement.IsMove), Movement.IsMove);
    }

    public void UpdateModel(GameObject model)
    {
        Model = Instantiate(model, PredictedObject.GetGraphicalObject());
        Model.gameObject.SetActive(!base.IsOwner);

        _playerAnimator = Model.GetComponent<Animator>();
    }
}