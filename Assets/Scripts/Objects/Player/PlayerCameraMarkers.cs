using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerCameraMarkers : MonoBehaviour
{
    [SerializeField] private List<TargetWaypoint> targetWaypoints = new List<TargetWaypoint>();

    private void Start()
    {
        AddWaypointsItemsToList(PlayerUIMarkers.Instance.Markers.ToList());
    }

    private void AddWaypointsItemsToList(List<Marker> markers)
    {
        for (int i = 0; i < targetWaypoints.Count; i++)
        {
            targetWaypoints[i].SetMarker(markers[i]);
        }
    }
}
