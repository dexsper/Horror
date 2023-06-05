using UnityEngine;

public class MarkerTarget : MonoBehaviour
{
    private Marker _marker;
    private Generator _generator;

    public Marker Marker => _marker;
    
    private void Awake()
    {
        if (GetComponent<Generator>())
        {
            _generator = GetComponent<Generator>();
        }
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
        if (_marker != null && repairedGenerator == _generator)
        {
            _marker.DisableMarker();
        }
    }
}
