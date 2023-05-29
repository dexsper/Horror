using System.Collections;
using System.Collections.Generic;
using CAS.AdObject;
using UnityEngine;

public class Banner : MonoBehaviour
{
    private BannerAdObject _banner;

    private void Awake()
    {
        _banner = BannerAdObject.Instance;
    }

    private void Start()
    {
        _banner.gameObject.SetActive(true);
        AnalyticsEventManager.OnEvent("Entered the game","enter","1");
    }
}
