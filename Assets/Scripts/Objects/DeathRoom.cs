using System;
using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

public class DeathRoom : NetworkBehaviour
{
    [SerializeField] private int _leaveProgressAmount;
    [SerializeField] private List<Transform> _spawns;
    [SerializeField] private GameObject _interface;

    private static DeathRoom _instance;
    private int _nextSpawn = 0;
    private Dictionary<PlayerBehavior, int> _leaveProgress = new Dictionary<PlayerBehavior, int>();

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
    public static event Action<PlayerBehavior> OnPlayerLeave;

    private void Awake()
    {
        Instance = this;
    }

    [Server]
    public void AddPlayer(PlayerBehavior player)
    {
        _leaveProgress.Add(player, 0);

        GetSpawn(out Vector3 position, out Quaternion rotation);
        player.transform.SetPositionAndRotation(position, rotation);

        RPC_SetInterfaceActive(player.Owner, true);
    }

    [Server]
    private void RemovePlayer(PlayerBehavior player)
    {
        RPC_SetInterfaceActive(player.Owner, false);

        OnPlayerLeave?.Invoke(player);
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddLeaveProgress(PlayerBehavior player)
    {
        _leaveProgress[player]++;

        if (_leaveProgress[player] >= _leaveProgressAmount)
        {
            _leaveProgress.Remove(player);

            RemovePlayer(player);
        }
    }

    [TargetRpc]
    private void RPC_SetInterfaceActive(NetworkConnection conn, bool active)
    {
        _interface.gameObject.SetActive(active);
    }

    private void GetSpawn(out Vector3 pos, out Quaternion rot)
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
