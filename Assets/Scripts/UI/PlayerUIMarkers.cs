using UnityEngine;

public class PlayerUIMarkers : MonoBehaviour
{
    [SerializeField] private Marker _markerPrefab;
    [SerializeField] private Transform _markersParent;

    private void Start()
    {
        var markerTargets = FindObjectsOfType<MarkerTarget>();

        for (int i = 0; i < markerTargets.Length; i++)
        {
            var marker = Instantiate(_markerPrefab, _markersParent);
            marker.SetTarget(markerTargets[i]);
        }
    }
}