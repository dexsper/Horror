using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class LeaveZone : NetworkBehaviour
{
    public static List<LeaveZone> LeaveZones = new List<LeaveZone>();

    private MarkerTarget _markerTarget;
    
    public override void OnStartNetwork()
    {
        base.OnStartNetwork();

        SetActive_RPC(false);

        LeaveZones.Add(this);

        _markerTarget = GetComponent<MarkerTarget>();
        _markerTarget.Marker.gameObject.SetActive(false);
    }

    [Server]
    public void Activate()
    {
        gameObject.SetActive(true);
        _markerTarget.Marker.gameObject.SetActive(true);
        
        SetActive_RPC(true);
    }

    [ObserversRpc(RunLocally = true, BufferLast = true)]
    private void SetActive_RPC(bool active)
    {
        gameObject.SetActive(active);
    }
}