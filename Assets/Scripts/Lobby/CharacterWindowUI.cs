using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using Unity.Services.Economy;
using Unity.Services.Economy.Model;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class CharacterWindowUI : MonoBehaviour
{
    [Title("Data", TitleAlignment = TitleAlignments.Centered)]
    [SerializeField] private CharactersData _characters;

    [Title("Interface", TitleAlignment = TitleAlignments.Centered)]
    [SerializeField] private TextMeshProUGUI _playerCurrentMoney;
    [SerializeField] private CharacterUI _characterViewPrefab;
    [SerializeField] private Transform _charactersContent;
    [SerializeField] private Button _closeButton;

    [SerializeField] private GameObject editPlayerName, authentificate,gameAdObject;

    [SerializeField] private string rewardedSkinID;

    private static CharacterWindowUI _instance;
    private Dictionary<string, CharacterUI> _spawnedCharacters;
    private PlayerBalance _playerBalance;

    public static CharacterWindowUI Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<CharacterWindowUI>();
            }

            return _instance;
        }

        set => _instance = value;
    }
    public event Action OnCharacterSelected;

    public static event Action OnRewardedSkinAdded;
    public string SelectedCharacterName { get; private set; }

    private void Awake()
    {
        if (Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;

        PlayerEconomy.OnDataRefreshed += UpdateCharacters;
        PlayerEconomy.OnDataRefreshed += GetPlayerBalance;
        PlayerEconomy.OnDataRefreshed += UpdateUI;
        
        _closeButton.onClick.AddListener(CloseWindow);
    }
    
    private void Start()
    {
        CloseWindow();
    }

    public void OpenWindow()
    {
        gameObject.SetActive(true);
    }
    
    private void CloseWindow()
    {
        gameObject.SetActive(false);
        gameAdObject.SetActive(true);
        authentificate.SetActive(true);
        editPlayerName.SetActive(true);
    }
    private void OnDestroy()
    {
        PlayerEconomy.OnDataRefreshed -= UpdateCharacters;
        PlayerEconomy.OnDataRefreshed -= GetPlayerBalance;
        PlayerEconomy.OnDataRefreshed -= UpdateUI;
    }

    private async void GetPlayerBalance()
    {
        _playerBalance = await PlayerEconomy.Instance.CurrencyDefinition.GetPlayerBalanceAsync();
        _playerCurrentMoney.text = $"{_playerBalance.Balance}";
    }

    public void ShowRewardedAdd()
    {
        Ads.Instance.ShowRewardedAd(OnRewardedAdPurchase);
    }

    private async void OnRewardedAdPurchase()
    {
        AnalyticsEventManager.OnEvent("Rewarded Ad closed", "rewardAd","1");
        PlayersInventoryItem createdInventoryItem =
            await EconomyService.Instance.PlayerInventory.AddInventoryItemAsync(rewardedSkinID);
        PlayerEconomy.Instance.Refresh();
        OnRewardedSkinAdded?.Invoke();
    }
    
    public void CharacterAction(string characterName)
    {
        if (CharacterIsPurchased(characterName))
        {
            SelectedCharacterName = characterName;
            OnCharacterSelected?.Invoke();

            gameObject.SetActive(false);
        }
        else
        {
            var itemDefention = PlayerEconomy.Instance.InventoryDefinitions.First(x => x.Name == characterName);

            PlayerEconomy.Instance.MakePurchase(itemDefention.Id);
            AnalyticsEventManager.OnEvent("Buy skin","buyed",characterName);
        }
    }

    private void UpdateCharacters()
    {
        if (_spawnedCharacters != null)
        {
            UpdateUI();

            return;
        }

        _spawnedCharacters = new Dictionary<string, CharacterUI>();
        var charactersDefinitions = PlayerEconomy.Instance.InventoryDefinitions.Select(x => x.Name).ToList();

        foreach (var (characterName, character) in _characters.Characters)
        {
            if (charactersDefinitions.Contains(characterName))
            {
                var characterUI = Instantiate(_characterViewPrefab, _charactersContent);

                var itemDefention = PlayerEconomy.Instance.InventoryDefinitions.First(x => x.Name == characterName);
                var price = PlayerEconomy.Instance.GetItemPrice(itemDefention.Id);

                characterUI.Initialize(characterName, price, _characters[characterName]);

                _spawnedCharacters.Add(characterName, characterUI);
            }
        }

        if (PlayerEconomy.Instance.InventoryItems.Count > 0)
        {
            string firstBuyedCharacter = PlayerEconomy.Instance.InventoryItems[0].GetItemDefinition().Name;

            SelectedCharacterName = firstBuyedCharacter;
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        foreach (var (_, characterUI) in _spawnedCharacters)
        {
            characterUI.UpdateUI();
        }
    }

    public bool CharacterIsPurchased(string name)
    {
        bool purchased = PlayerEconomy.Instance.InventoryItems.Exists((x) =>
        {
            var defention = x.GetItemDefinition();

            return defention.Name == name;
        });

        return purchased;
    }
}
