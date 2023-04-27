using FishNet.Object;
using Sirenix.OdinInspector;
using UnityEngine;

public class ManiacBehaviour : NetworkBehaviour
{
    public Vector2 Movement;

    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    [Server]
    private void Update()
    {
        if (!IsServer)
            return;

        _rb.velocity = new Vector3(Movement.x, 0f, -Movement.y);
    }
}
