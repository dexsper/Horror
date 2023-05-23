using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIMarkers : MonoBehaviour
{
    public static PlayerUIMarkers Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        AddGenerators();
    }

    [SerializeField] private List<Marker> markers = new List<Marker>();

    public IReadOnlyList<Marker> Markers => markers;

    private void AddGenerators()
    {
        for (int i = 0; i < markers.Count; i++)
        {
            markers[i].Generator = Generator.Generators[i];
        }
    }
}

[Serializable]
public class Marker
{
    public Image MarkerImage;
    public TextMeshProUGUI MarkerDistance;
    public Generator Generator;
}