using System;
using CAS;
using UnityEngine;

public class Ads : MonoBehaviour
{
    #region Singleton

    public static Ads Instance;
    private void Awake()
    {
        Instance = this;
    }

    #endregion
    
    public IMediationManager manager { get; set; }

    private IAdView adViewAdaptive;
    private void Start()
    {
        Init();
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }
    private void Init()
    {
        manager = MobileAds.BuildManager()
            .WithConsentFlow(new ConsentFlow().WithPrivacyPolicy("https://hadievv2048.wixsite.com/khadievpp"))
            .WithManagerIdAtIndex(0)
            .WithInitListener((success, error) =>
            {
            })
            .WithMediationExtras(MediationExtras.facebookDataProcessing, "LDU")
            .Initialize();

        MobileAds.settings.allowInterstitialAdsWhenVideoCostAreLower = true;
        MobileAds.settings.isExecuteEventsOnUnityThread = true;
        MobileAds.settings.analyticsCollectionEnabled = true;
        manager.SetAppReturnAdsEnabled(true);

        adViewAdaptive = manager.GetAdView(AdSize.AdaptiveFullWidth);
        adViewAdaptive.SetActive(true);

        adViewAdaptive.position = AdPosition.TopCenter;
    }

    private void CheckConnection()
    {
        
    }
    
    public void ShowAd()
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            if (manager.IsReadyAd(AdType.Interstitial))
            {
                manager.ShowAd(AdType.Interstitial);
                InterstitialAndRewardedEvent();
                Debug.Log("Ad showed");
            }
            else
            {
                Debug.Log("Ad can not be showed");
            }
        }
    }
    
    private void InterstitialAndRewardedEvent()
    {
        if (PlayerPrefs.HasKey("InterstitialAndRewarded"))
        {
            PlayerPrefs.SetInt("InterstitialAndRewarded", PlayerPrefs.GetInt("InterstitialAndRewarded") + 1);
        }
        else
        {
            PlayerPrefs.SetInt("InterstitialAndRewarded", 1);
        }
        AnalyticsEventManager.OnEvent("InterstitialAndRewarded", "whatCount", PlayerPrefs.GetInt("InterstitialAndRewarded").ToString());
    }
}
