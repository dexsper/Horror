using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class DeathRoom : NetworkBehaviour
{
    [SerializeField] private List<Transform> _spawns;

    private static DeathRoom _instance;
    private int _nextSpawn = 0;

    public static DeathRoom Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<DeathRoom>();
            }

            return _instance;
        }

        private set => _instance = value;
    }

    private void Awake()
    {
        Instance = this;
    }

    [Server]
    public void AddPlayer(PlayerBehavior player)
    {
        SetSpawn(out Vector3 position, out Quaternion rotation);
        player.transform.SetPositionAndRotation(position, rotation);
    }

    private void SetSpawn(out Vector3 pos, out Quaternion rot)
    {
        if (_spawns.Count == 0)
        {
            pos = Vector3.zero;
            rot = Quaternion.identity;

            return;
        }

        Transform result = _spawns[_nextSpawn];

        pos = result.position;
        rot = result.rotation;

        _nextSpawn = (_nextSpawn + 1) % _spawns.Count;
    }
}
