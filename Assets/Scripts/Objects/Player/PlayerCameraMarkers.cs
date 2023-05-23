using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraMarkers : MonoBehaviour
{
    [SerializeField] private List<TargetWaypoint> targetWaypoints = new List<TargetWaypoint>();

    public IReadOnlyList<TargetWaypoint> TargetWaypoints => targetWaypoints;
    public void AddWaypointsItemsToList(List<Marker> markers)
    {
        for (int i = 0; i < targetWaypoints.Count; i++)
        {
            targetWaypoints[i].SetImage(markers[i].MarkerImage);
            targetWaypoints[i].SetText(markers[i].MarkerDistance);
            targetWaypoints[i].SetTarget(markers[i].Generator.transform);
        }
    }
}
