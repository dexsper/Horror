using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class LeaveZone : NetworkBehaviour
{
    public static List<LeaveZone> LeaveZones = new List<LeaveZone>();

    public override void OnStartNetwork()
    {
        base.OnStartNetwork();

        SetActive_RPC(false);

        LeaveZones.Add(this);
    }

    [Server]
    public void Activate()
    {
        gameObject.SetActive(true);

        SetActive_RPC(true);
    }

    [ObserversRpc(RunLocally = true, BufferLast = true)]
    private void SetActive_RPC(bool active)
    {
        gameObject.SetActive(active);
    }
}