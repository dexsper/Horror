using UnityEngine;
using Zenject;

public class PlayerInput : MonoBehaviour, IPlayerInput
{
    [Inject] private Joystick _joystick;
    [Inject] private TouchField _touchField;
    [SerializeField] private Vector2 lookSensitivity;

    public Vector2 Movement { get; private set; }
    public Vector2 Look { get; private set; }

    private void Update()
    {
        if (_joystick == null && _touchField == null)
            return;

        lookSensitivity = new Vector2(PlayerPrefs.GetFloat("Sensitivity"),2);
        Movement = _joystick.Direction;
        
#if UNITY_EDITOR
        Movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
#endif

       Look = _touchField.TouchDist * lookSensitivity;
    }
}
