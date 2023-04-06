using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterWindowUI : MonoBehaviour
{
    [SerializeField] private List<Character> characters = new List<Character>();

    [SerializeField] private Button nextButton, prevButton;
    [SerializeField] private TextMeshProUGUI characterNameText;

    private int _nextCharacter = 0;

    private void Awake()
    {
        nextButton.onClick.AddListener(NextCharacter);
        UpdateUI(0);
    }

    private void NextCharacter()
    {
        _nextCharacter = (_nextCharacter + 1) % characters.Count;
        UpdateUI(_nextCharacter);
    }

    private void PreviousCharacter()
    {
        
    }

    private void UpdateUI(int index)
    {
        foreach (var character in characters)
        {
            character.Model.gameObject.SetActive(false);
        }
        characterNameText.text = $"{characters[index].Name}";
        characters[index].Model.SetActive(true);
    }
}

[Serializable]
public class Character
{
    [SerializeField] private string name;
    [SerializeField] private GameObject model;
    public string Name => name;
    public GameObject Model => model;
}
