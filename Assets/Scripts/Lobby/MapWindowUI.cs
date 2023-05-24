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

    public static MapWindowUI Instance { get; private set; }
    public string SelectedMap { get; private set; }

    private void Awake()
    {
        nextButton.onClick.AddListener(NextMap);
        prevButton.onClick.AddListener(PreviousMap);

        prevButton.interactable = LobbyManager.Instance.JoinedLobby == null || LobbyManager.Instance.IsLobbyHost();
        nextButton.interactable = LobbyManager.Instance.JoinedLobby == null || LobbyManager.Instance.IsLobbyHost();

        Instance = this;

        UpdateMap(maps[0].MapName);
    }

    public void SetMapNameText(string mapName)
    {
        mapNameText.text = mapName;
    }

    private void NextMap()
    {
        _nextMap = (_nextMap + 1) % maps.Count;

        UpdateMap(maps[_nextMap].MapName);
    }

    private void PreviousMap()
    {
        _nextMap--;

        if (_nextMap < 0)
            _nextMap = maps.Count - 1;

        UpdateMap(maps[_nextMap].MapName);
    }

    public void UpdateMap(string mapName)
    {
        if (LobbyManager.Instance.JoinedLobby != null && LobbyManager.Instance.GetMap() != mapName)
            LobbyManager.Instance.UpdateLobbyMap(mapName);

        SelectedMap = mapName;
        mapNameText.text = mapName;

        var index = 0;

        for (int i = 0; i < maps.Count; i++)
        {
            if (maps[i].MapName == mapName)
            {
                index = i;
            }
        }

        mapImage.sprite = maps[index].MapSprite;
    }
}

[Serializable]
public class Map
{
    [SerializeField] private Sprite mapSprite;
    [SerializeField] private string mapName;

    public Sprite MapSprite => mapSprite;
    public string MapName => mapName;
}
