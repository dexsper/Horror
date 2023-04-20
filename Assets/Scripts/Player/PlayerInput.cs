using System;
using UnityEngine;
using Zenject;

public class PlayerInput : MonoBehaviour, IPlayerInput
{
    [Inject] private Joystick _joystick;
    [Inject] private TouchField _touchField;
    [SerializeField] private Vector2 lookSensitivity;

    private PlayerMovement _playerMovement;
    public Vector2 Movement { get; private set; }
    public Vector2 Look { get; private set; }

    private void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (_joystick == null && _touchField == null)
            return;

        Movement = _joystick.Direction;
        Look = _touchField.TouchDist * lookSensitivity;

#if UNITY_EDITOR
        Movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
#endif
    }
}
