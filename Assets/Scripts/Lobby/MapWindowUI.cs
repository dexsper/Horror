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
    private LobbyManager _lobbyManager;

    public void SetMapNameText(string mapName)
    {
        mapNameText.text = mapName;
    }

    public void NextButtonStatus(bool status)
    {
        nextButton.gameObject.SetActive(status);
    }
    
    private void Awake()
    {
        nextButton.onClick.AddListener(NextCharacter);
        prevButton.onClick.AddListener(PreviousCharacter);
        if (LobbyManager.Instance.IsLobbyHost())
        {
            nextButton.interactable = true;
        }
    }

    private void NextCharacter()
    {
        _nextMap = (_nextMap + 1) % maps.Count;
        UpdateUI(_nextMap);
    }

    private void PreviousCharacter()
    {
        _nextMap--;

        if (_nextMap < 0)
            _nextMap = maps.Count - 1;

        UpdateUI(_nextMap);
    }

    private void UpdateUI(int index)
    {
        LobbyManager.Instance.UpdateLobbyMap(maps[index].MapName);
        mapNameText.text = $"{LobbyManager.Instance.GetMap()}";
    }
    
    public void SetCurrentMapSprite(string mapName)
    {
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
