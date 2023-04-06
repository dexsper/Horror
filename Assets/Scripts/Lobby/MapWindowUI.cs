using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object.Synchronizing;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MapWindowUI : MonoBehaviour
{
    [SerializeField] private List<Map> maps = new List<Map>();

    [SerializeField] private Button nextButton, prevButton;
    [SerializeField] private TextMeshProUGUI mapNameText;
    [SerializeField] private Image mapImage;

    private int _nextMap = 0;
    
    private void Awake()
    {
        nextButton.onClick.AddListener(NextCharacter);
        UpdateUI(0);
    }

    private void NextCharacter()
    {
        _nextMap = (_nextMap + 1) % maps.Count;
        UpdateUI(_nextMap);
    }

    private void PreviousCharacter()
    {
        
    }

    private void UpdateUI(int index)
    {
        mapNameText.text = $"{maps[index].MapName}";
        mapImage.sprite = maps[index].MapSprite;
    }
}
[Serializable    ]
public class Map
{
    [SerializeField] private Sprite mapSprite;
    [SerializeField] private string mapName;

    public Sprite MapSprite => mapSprite;
    public string MapName => mapName;
}
