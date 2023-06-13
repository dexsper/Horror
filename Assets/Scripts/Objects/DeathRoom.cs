using System;
using System.Collections.Generic;
using System.Linq;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Sirenix.OdinInspector;
using UnityEngine;

public class DeathRoom : NetworkBehaviour
{
    [SerializeField] private float _timeToLeft = 5f;

    [Title("Interface")]
    [SerializeField] private DeathRoomUI _ui;

    [Title("Room")]
    [SerializeField] private List<Transform> _spawns;

    private static DeathRoom _instance;
    private int _nextSpawn = 0;
    [SyncObject] private readonly SyncDictionary<PlayerBehavior, float> _leaveProgress = new();

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

        _leaveProgress.OnChange += OnLeaveProgressChange;
    }

    private void OnLeaveProgressChange(SyncDictionaryOperation op, PlayerBehavior key, float value, bool asServer)
    {
        if (asServer)
            return;

        if (key != PlayerBehavior.LocalPlayer)
            return;

        _ui.UpdateProgress(value);
    }

    [Server]
    private void Update()
    {
        var keys = _leaveProgress.Keys.ToList();

        foreach (var key in keys)
        {
            _leaveProgress[key] -= Time.deltaTime;

            if (_leaveProgress[key] <= 0)
            {
                _leaveProgress.Remove(key);
                RemovePlayer(key);
            }
        }
    }

    [Server]
    public void AddPlayer(PlayerBehavior player)
    {
        _leaveProgress.Add(player, _timeToLeft);

        GetSpawn(out Vector3 position, out Quaternion rotation);
        player.transform.SetPositionAndRotation(position, rotation);

        RPC_SetInterfaceActive(player.Owner, true);
    }

    [Server]
    private void RemovePlayer(PlayerBehavior player)
    {
        RPC_SetInterfaceActive(player.Owner, false);

        AnalyticsEventManager.OnEvent("Leave jail", "Jail Leave", "1");
        Ads.Instance.ShowAd();
        OnPlayerLeave?.Invoke(player);
    }

    [TargetRpc]
    private void RPC_SetInterfaceActive(NetworkConnection conn, bool active)
    {
        _ui.gameObject.SetActive(active);
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
