using System;
using System.Collections.Generic;
using System.Linq;
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
    [field: ShowInInspector, ReadOnly] public List<PlayersInventoryItem> InventoryItems { get; private set; }
    [field: ShowInInspector, ReadOnly] public List<VirtualPurchaseDefinition> VirtualPurchases { get; private set; }

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

    public async void Refresh()
    {
        await EconomyService.Instance.Configuration.SyncConfigurationAsync();

        CurrencyDefinition = EconomyService.Instance.Configuration.GetCurrency(CURRENCY_ID);
        InventoryDefinitions = EconomyService.Instance.Configuration.GetInventoryItems();
        VirtualPurchases = EconomyService.Instance.Configuration.GetVirtualPurchases();

        GetInventoryResult inventoryResult = await EconomyService.Instance.PlayerInventory.GetInventoryAsync();
        InventoryItems = inventoryResult.PlayersInventoryItems;

        if (InventoryItems.Count == 0)
        {
            var balanceInfo = await CurrencyDefinition.GetPlayerBalanceAsync();

            if (balanceInfo.Balance == 0)
            {
                MakePurchase(DEFAULT_ID);
            }
        }

        OnDataRefreshed?.Invoke();
    }

    public async void MakePurchase(string id)
    {
        string purchaseID = $"{id}_PURCHASE";

        try
        {
            MakeVirtualPurchaseResult purchaseResult = await EconomyService.Instance.Purchases.MakeVirtualPurchaseAsync(purchaseID);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

        Refresh();
    }

    public async void AddItem(string id)
    {
        await EconomyService.Instance.PlayerInventory.AddInventoryItemAsync(id);
        Refresh();
    }
    

    public int GetItemPrice(string id)
    {
        string purchaseID = $"{id}_PURCHASE";
        var purchase = VirtualPurchases.FirstOrDefault(p => p.Id == purchaseID);

        if (purchase != null)
        {
            var moneyCost = purchase.Costs.FirstOrDefault((c) =>
            {
                var itemDefention = c.Item.GetReferencedConfigurationItem();

                return itemDefention.Id == CurrencyDefinition.Id;
            });

            if (moneyCost != null)
                return moneyCost.Amount;
        }

        return 0;
    }

    public async void IncrementBalance(int amount)
    {
        await EconomyService.Instance.PlayerBalances.IncrementBalanceAsync(CurrencyDefinition.Id, amount);
    }
}
