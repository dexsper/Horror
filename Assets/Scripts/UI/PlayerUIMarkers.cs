using System.Collections.Generic;
using UnityEngine;

public class PlayerUIMarkers : MonoBehaviour
{
    [SerializeField] private Marker _markerPrefab;
    [SerializeField] private Transform _markersParent;

    [SerializeField] private List<MarkerTarget> _markerTargets = new List<MarkerTarget>();
    
    private void Start()
    {
        //SpawnMarkers();
    }

    private void SpawnMarkers()
    {
        for (int i = 0; i < _markerTargets.Count; i++)
        {
            var marker = Instantiate(_markerPrefab, _markersParent);
            marker.SetTarget(_markerTargets[i]);
        }
    }
}