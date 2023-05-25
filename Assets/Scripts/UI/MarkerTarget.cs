using System;
using UnityEngine;

public class MarkerTarget : MonoBehaviour
{
    private Marker _marker;
    private Generator _generator;

    private void Awake()
    {
        if(GetComponent<Generator>())
            _generator = GetComponent<Generator>();
        Generator.OnRepaired += OnGeneratorRepaired;
    }

    private void OnDestroy()
    {
        Generator.OnRepaired -= OnGeneratorRepaired;
    }

    public void SetMarker(Marker marker)
    {
        _marker = marker;
    }

    private void OnGeneratorRepaired(Generator repairedGenerator)
    {
        if (repairedGenerator == _generator)
        {
            _marker.DisableMarker();
        }
    }
}
