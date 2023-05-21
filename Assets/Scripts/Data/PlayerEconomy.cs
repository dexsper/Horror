using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.Services.Authentication;
using Unity.Services.Economy;
using Unity.Services.Economy.Model;
using UnityEngine;

public class PlayerEconomy : MonoBehaviour
{
    public static string DEFAULT_ID = "DEFAULT";
    public const string CURRENCY_ID = "INGAME_MONEY";

    [field: Title("Services Configuration", TitleAlignment = TitleAlignments.Centered)]
    [field: ShowInInspector, ReadOnly] public CurrencyDefinition CurrencyDefinition { get; private set; }
    [field: ShowInInspector, ReadOnly] public List<InventoryItemDefinition> InventoryDefinitions { get; private set; }
    [field: ShowInInspector, ReadOnly] public List<PlayersInventoryItem> PlayersInventoryItems { get; private set;}

    public static PlayerEconomy Instance { get; private set; }
    public static event Action OnDataRefreshed;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;

        LobbyManager.OnServicesInitialized += Initialize;
    }

    private void Initialize()
    {
        AuthenticationService.Instance.SignedIn += Refresh;
    }

    private async void Refresh()
    {
        await EconomyService.Instance.Configuration.SyncConfigurationAsync();

        CurrencyDefinition = EconomyService.Instance.Configuration.GetCurrency(CURRENCY_ID);
        InventoryDefinitions = EconomyService.Instance.Configuration.GetInventoryItems();

        GetInventoryResult inventoryResult = await EconomyService.Instance.PlayerInventory.GetInventoryAsync();
        PlayersInventoryItems = inventoryResult.PlayersInventoryItems;

        if(PlayersInventoryItems.Count == 0)
        {
            var balanceInfo = await CurrencyDefinition.GetPlayerBalanceAsync();

            if(balanceInfo.Balance == 0)
            {
                MakePurchase(DEFAULT_ID);
            }
        }

        OnDataRefreshed?.Invoke();
    }

    public async void MakePurchase(string id)
    {
        string purchaseID = $"{id}_PURCHASE";

        MakeVirtualPurchaseResult purchaseResult = await EconomyService.Instance.Purchases.MakeVirtualPurchaseAsync(purchaseID);

        Refresh();
    }

    public async void IncrementBalance(int amount)
    {
        await EconomyService.Instance.PlayerBalances.IncrementBalanceAsync(CurrencyDefinition.Id, amount);
    }
}
