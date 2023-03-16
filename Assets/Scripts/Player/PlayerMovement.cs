using System;
using UnityEngine;
using System.Collections.Generic;
using Mirror;

public class PlayerMovement : MonoBehaviour
{
    private float _trueSpeed;
    [SerializeField] private float speed = 5;
    [SerializeField] private float runSpeed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private Joystick playerJoystick;

    private Vector2 _velocity;
    private Transform _playerTransform;
    private Transform _playerCamera;
    private float _horizontal, _vertical;
    private Rigidbody _playerRb;
    private PlayerAnimator _playerAnimator;

    public bool IsRunning { get; private set; }

    public void Run()
    {
        _trueSpeed = runSpeed;
        IsRunning = true;
    }

    public void CancelRun()
    {
        _trueSpeed = speed;
        _playerRb.velocity = Vector3.zero;
        IsRunning = false;
    }

    private void Start()
    {
        _trueSpeed = speed;
        _playerRb = GetComponent<Rigidbody>();
        _playerCamera = Camera.main.transform;
        _playerTransform = gameObject.transform;
        _playerAnimator = GetComponent<PlayerAnimator>();
    }

    private void Update()
    {
        if (!PlayerComponents.Instance.isLocalPlayer)
        {
            return;
        }

        _playerAnimator.SetBlendParameter(-_horizontal, _horizontal, _vertical, -_vertical);
        if (IsRunning)
        {
            playerJoystick.enabled = false;
            _playerRb.AddForce(transform.forward * _trueSpeed);
            if (_playerRb.velocity.magnitude >= maxSpeed)
            {
                _playerRb.velocity = _playerRb.velocity.normalized * maxSpeed;
            }

            //_playerTransform.Translate(Vector3.forward * _velocity.x);
            _playerAnimator.StartRunAnimation();
        }
        else
        {
            playerJoystick.enabled = true;
            _playerAnimator.CancelRunAnimation();
        }
    }

    void FixedUpdate()
    {
        if (!PlayerComponents.Instance.isLocalPlayer)
        {
            return;
        }
        
        _playerTransform.rotation = Quaternion.Euler(_playerTransform.rotation.eulerAngles.x,
            _playerCamera.rotation.eulerAngles.y, _playerTransform.rotation.eulerAngles.z);

        _horizontal = playerJoystick.Horizontal;
        _vertical = playerJoystick.Vertical;

        if (_horizontal != 0 || _vertical != 0)
        {
            _playerAnimator.StartWalkAnimation();
        }
        else
        {
            _playerAnimator.CancelWalkAnimation();
        }

        _velocity.y = _vertical * _trueSpeed * Time.deltaTime;
        _velocity.x = _horizontal * _trueSpeed * Time.deltaTime;
        _playerTransform.Translate(_velocity.x, 0, _velocity.y);
    }
}