using System;
using UnityEngine;
using Zenject;

public class PlayerInput : MonoBehaviour, IPlayerInput
{
    [Inject] private Joystick _joystick;

    public Vector2 Movement { get; private set; }
    public Vector2 LookDirection { get; private set; }

    private void Update()
    {
        if (_joystick == null) 
            return;
        
        Movement = _joystick.Direction;
    }
}
