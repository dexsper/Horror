using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.Services.Authentication;
using Unity.Services.Economy;
using Unity.Services.Economy.Model;
using UnityEngine;

public class PlayerEconomy : MonoBehaviour
{
    public const string CURRENCY_ID = "INGAME_MONEY";

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

        OnDataRefreshed?.Invoke();
    }

    public async void MakePurchase(InventoryItemDefinition item)
    {
        string purchaseID = $"{item.Id}_PURCHASE";

        MakeVirtualPurchaseResult purchaseResult = await EconomyService.Instance.Purchases.MakeVirtualPurchaseAsync(purchaseID);

        Refresh();
    }

    public async void IncrementBalance(int amount)
    {
        await EconomyService.Instance.PlayerBalances.IncrementBalanceAsync(CurrencyDefinition.Id, amount);
    }
}
