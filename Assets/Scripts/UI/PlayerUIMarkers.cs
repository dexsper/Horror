using System;
using System.Collections;
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

    [SerializeField] private List<Marker> markers = new List<Marker>();

    public List<Marker> GetList()
    {
        return markers;
    }

    public void AddGenerators()
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