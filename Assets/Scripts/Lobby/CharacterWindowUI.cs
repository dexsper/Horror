using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
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
    [SerializeField] private TextMeshProUGUI _playerCurrentMoney;
    [SerializeField] private CharacterUI _characterViewPrefab;
    [SerializeField] private Transform _charactersContent;
    [SerializeField] private Button _closeButton;

    [SerializeField] private GameObject editPlayerName, authentificate,gameAdObject,multiplayerSettingsButton;

    [SerializeField] private string rewardedSkinID;

    [SerializeField] private List<GameObject> objectsToDisable;
    
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

        private set => _instance = value;
    }
    public event Action OnCharacterSelected;
    public event Action OnShopOpened;
    public event Action OnShopClosed;
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

    private void RefreshWindow()
    {
        GetPlayerBalance();
        UpdateUI();
    }

    public void OpenWindow()
    {
        gameObject.transform.DOScale(1f, 0.2f).SetEase(Ease.Linear);
        _charactersContent.gameObject.SetActive(true);
        multiplayerSettingsButton.SetActive(false);
        _closeButton.gameObject.SetActive(true);
        OnShopOpened?.Invoke();
    }
    
    private void CloseWindow()
    {
        OnShopClosed?.Invoke();
        multiplayerSettingsButton.SetActive(true);
        gameObject.transform.DOScale(0f, 0.2f).SetEase(Ease.Linear);
        _charactersContent.gameObject.SetActive(false);
        gameAdObject.SetActive(true);
        authentificate.SetActive(true);
        editPlayerName.SetActive(true);
        for (int i = 0; i < objectsToDisable.Count; i++)
        {
            objectsToDisable[i].transform.DOScale(1f, 0.2f).SetEase(Ease.Linear);
        }
    }
    private void OnDestroy()
    {
        PlayerEconomy.OnDataRefreshed -= UpdateCharacters;
        PlayerEconomy.OnDataRefreshed -= GetPlayerBalance;
        PlayerEconomy.OnDataRefreshed -= UpdateUI;
    }

    private async void GetPlayerBalance()
    {
        if (PlayerEconomy.Instance == null)
            return;
        
        _playerBalance = await PlayerEconomy.Instance.CurrencyDefinition.GetPlayerBalanceAsync();
        _playerCurrentMoney.text = $"{_playerBalance.Balance}";
    }

    public void ShowRewardedAdd()
    {
        Ads.Instance.ShowRewardedAd(OnRewardedAdPurchase);
    }

    private void OnRewardedAdPurchase()
    {
        PlayerEconomy.Instance.AddItem(rewardedSkinID);
        AnalyticsEventManager.OnEvent("Rewarded Ad closed", "rewardAd","1");
    }
    
    public void CharacterAction(string characterName)
    {
        if (CharacterIsPurchased(characterName))
        {
            SelectedCharacterName = characterName;
            OnCharacterSelected?.Invoke();

            for (int i = 0; i < objectsToDisable.Count; i++)
            {
                objectsToDisable[i].transform.DOScale(1f, 0.2f).SetEase(Ease.Linear);
            }
            gameObject.transform.DOScale(0f, 0.2f).SetEase(Ease.Linear);
            _charactersContent.gameObject.SetActive(false);
        }
        else
        {
            var itemDefinition = PlayerEconomy.Instance.InventoryDefinitions.First(x => x.Name == characterName);

            PlayerEconomy.Instance.MakePurchase(itemDefinition.Id);
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

                var itemDefinition = PlayerEconomy.Instance.InventoryDefinitions.First(x => x.Name == characterName);
                var price = PlayerEconomy.Instance.GetItemPrice(itemDefinition.Id);

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

    public bool CharacterIsPurchased(string characterName)
    {
        bool purchased = PlayerEconomy.Instance.InventoryItems.Exists((x) =>
        {
            var definition = x.GetItemDefinition();

            return definition.Name == characterName;
        });

        return purchased;
    }
}
