using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using Unity.Services.Economy.Model;
using UnityEngine;
using UnityEngine.UI;

public class CharacterWindowUI : MonoBehaviour
{
    [Title("Data", TitleAlignment = TitleAlignments.Centered)]
    [SerializeField] private CharactersData _characters;

    [Title("Interface", TitleAlignment = TitleAlignments.Centered)]
    [SerializeField] private TextMeshProUGUI _characterNameText;
    [SerializeField] private TextMeshProUGUI _characterActionText;
    [SerializeField] private TextMeshProUGUI _playerCurrentMoney;
    [SerializeField] private Button _actionButton;
    [SerializeField] private Button _nextButton, _prevButton;

    [Title("Characters Models", TitleAlignment = TitleAlignments.Centered)]
    [SerializeField] private Transform _previewParent;

    [Title("LobbyList", TitleAlignment = TitleAlignments.Centered)]
    [SerializeField] private LobbyListUI lobbyListUI;
    
    private int _selectedCharacter = 0;
    private Dictionary<string, GameObject> _spawnedCharacters;

    private PlayerBalance _playerBalance;
    public string SelectedCharacterName { get; private set; }

    private void Awake()
    {
        _nextButton.onClick.AddListener(NextCharacter);
        _prevButton.onClick.AddListener(PreviousCharacter);
        _actionButton.onClick.AddListener(OnActionButton);

        PlayerEconomy.OnDataRefreshed += UpdateCharacters;
        PlayerEconomy.OnDataRefreshed += GetPlayerBalance;
    }
    private void OnDestroy()
    {
        PlayerEconomy.OnDataRefreshed -= UpdateCharacters;
        PlayerEconomy.OnDataRefreshed -= GetPlayerBalance;
    }

    private async void GetPlayerBalance()
    {
        _playerBalance = await PlayerEconomy.Instance.CurrencyDefinition.GetPlayerBalanceAsync();
        _playerCurrentMoney.text = $"{_playerBalance.Balance}";
    }
    
    private void OnActionButton()
    {
        string characterName = _spawnedCharacters.ElementAt(_selectedCharacter).Key;

        if (CharacterIsPurchased(characterName))
        {
            SelectedCharacterName = characterName;

            UpdateUI();
        }
        else
        {
            var itemDefention = PlayerEconomy.Instance.InventoryDefinitions.First(x => x.Name == characterName);

            PlayerEconomy.Instance.MakePurchase(itemDefention.Id);
        }
        lobbyListUI.gameObject.SetActive(true);
        LobbyManager.Instance.RefreshLobbyList();
        gameObject.SetActive(false);
    }

    private void UpdateCharacters()
    {
        if (_spawnedCharacters != null)
        {
            UpdateUI();

            return;
        }

        _spawnedCharacters = new Dictionary<string, GameObject>();
        var charactersDefinitions = PlayerEconomy.Instance.InventoryDefinitions.Select(x => x.Name).ToList();

        foreach (var (characterName, character) in _characters.Characters)
        {
            if (charactersDefinitions.Contains(characterName))
            {
                _spawnedCharacters.Add(characterName, Instantiate(character, _previewParent));
            }
        }

        if (PlayerEconomy.Instance.PlayersInventoryItems.Count > 0)
        {
            string firstBuyedCharacter = PlayerEconomy.Instance.PlayersInventoryItems[0].GetItemDefinition().Name;

            SelectedCharacterName = firstBuyedCharacter;
            _selectedCharacter = _spawnedCharacters.Keys.ToList().IndexOf(firstBuyedCharacter);
        }

        UpdateUI();
    }

    private void NextCharacter()
    {
        _selectedCharacter = (_selectedCharacter + 1) % _spawnedCharacters.Count;

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
        foreach (var (_, character) in _spawnedCharacters)
        {
            character.gameObject.SetActive(false);
        }

        var characterInfo = _spawnedCharacters.ElementAt(_selectedCharacter);
        string characterName = characterInfo.Key;

        characterInfo.Value.SetActive(true);
        _characterNameText.text = characterName;

        if (CharacterIsPurchased(characterName))
        {
            if (SelectedCharacterName == characterName)
            {
                _actionButton.interactable = true;
                _characterActionText.text = "Selected";
            }
            else
            {
                _actionButton.interactable = true;
                _characterActionText.text = "Select";
            }
        }
        else
        {
            _actionButton.interactable = true;
            _characterActionText.text = "Buy";
        }
    }

    private bool CharacterIsPurchased(string name)
    {
        bool purchased = PlayerEconomy.Instance.PlayersInventoryItems.Exists((x) =>
        {
            var defention = x.GetItemDefinition();

            return defention.Name == name;
        });

        return purchased;
    }
}
