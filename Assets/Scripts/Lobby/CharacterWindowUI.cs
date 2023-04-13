using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterWindowUI : MonoBehaviour
{
    [Title("Data", TitleAlignment = TitleAlignments.Centered)]
    [SerializeField] private CharactersData _characters;

    [Title("Interface", TitleAlignment = TitleAlignments.Centered)]
    [SerializeField] private TextMeshProUGUI _characterNameText;
    [SerializeField] private Button _nextButton, _prevButton;

    [Title("Characters Models", TitleAlignment = TitleAlignments.Centered)]
    [SerializeField] private Transform _previewParent;

    private int _selectedCharacter = 0;
    private Dictionary<string, GameObject> _spawnedCharacters;

    public string SelectedCharacterName { get; private set; }

    private void Awake()
    {
        _nextButton.onClick.AddListener(NextCharacter);
        _prevButton.onClick.AddListener(PreviousCharacter);

        _spawnedCharacters = new Dictionary<string, GameObject>();

        foreach (var (characterName, character) in _characters.Characters)
        {
            _spawnedCharacters.Add(characterName, Instantiate(character, _previewParent));
        }

        UpdateUI();
    }

    private void NextCharacter()
    {
        _selectedCharacter = (_selectedCharacter + 1) % _characters.Characters.Count;

        UpdateUI();
    }

    private void PreviousCharacter()
    {
        _selectedCharacter--;

        if (_selectedCharacter < 0)
            _selectedCharacter = _spawnedCharacters.Count - 1;

        UpdateUI();
    }

    private void UpdateUI()
    {
        foreach (var (characterName, character) in _spawnedCharacters)
        {
            character.gameObject.SetActive(false);
        }

        var characterInfo = _spawnedCharacters.ElementAt(_selectedCharacter);

        SelectedCharacterName = characterInfo.Key;

        _characterNameText.text = characterInfo.Key;
        characterInfo.Value.SetActive(true);
    }
}
