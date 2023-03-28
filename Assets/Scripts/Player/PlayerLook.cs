using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] private Transform playerBody;
    [SerializeField] private float sensitivity = 100f;
    private float _xRototation;
    private bool _lockCursor;

    [SerializeField] private PlayerTouchField fixedTouchField;
    [HideInInspector] public Vector2 LockAxis;

    void Update()
    {
        LockAxis = fixedTouchField.TouchDist;

        float mouX = LockAxis.x * sensitivity * Time.deltaTime;
        float mouY = LockAxis.y * sensitivity * Time.deltaTime;

        _xRototation -= mouY;
        _xRototation = Mathf.Clamp(_xRototation, -90f, 35f);

        transform.localRotation = Quaternion.Euler(_xRototation, 0f, 0f);

        playerBody.Rotate(Vector3.up * mouX);
    }
}
