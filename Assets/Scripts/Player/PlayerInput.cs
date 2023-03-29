using UnityEngine;
using Zenject;

public class PlayerInput : MonoBehaviour, IPlayerInput
{
    [Inject] private Joystick _joystick;
    [Inject] private PlayerTouchField _touchField;
    [SerializeField] private Vector2 lookSensitivity;

    public Vector2 Movement { get; private set; }
    public Vector2 LookDirection { get; private set; }
    public bool IsSprint { get; private set; }

    private void Update()
    {
        if (_joystick == null && _touchField == null)
            return;

        Movement = _joystick.Direction;
        LookDirection = _touchField.TouchDist * lookSensitivity;
    }
}
