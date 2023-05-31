using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanelUI : MonoBehaviour
{
    [SerializeField] private float timeToOpen = 0.25f;

    [SerializeField] private Button openButton, closeButton;
    [SerializeField] private Button casButton;

    [SerializeField] private Transform warningPanel;
    [SerializeField] private Button confirmButton, cancelButton;

    private void Awake()
    {
        openButton.onClick.AddListener(OpenMenu);
        closeButton.onClick.AddListener(CloseMenu);
        
        casButton.onClick.AddListener(OpenWarningPanel);
        
        confirmButton.onClick.AddListener(Confirm);
        cancelButton.onClick.AddListener(Cancel);
    }

    private void OpenMenu()
    {
        transform.DOScale(1f, timeToOpen).From(0f).SetEase(Ease.Linear);
    }

    private void CloseMenu()
    {
        transform.DOScale(0f, timeToOpen).From(1f).SetEase(Ease.Linear);
    }

    public void OpenURL(string url)
    {
        Application.OpenURL(url);
    }

    private void OpenWarningPanel()
    {
        warningPanel.DOScale(1f, timeToOpen).From(0).SetEase(Ease.Linear);
    }

    private void CloseWarningPanel()
    {
        warningPanel.DOScale(0f, timeToOpen).From(1f).SetEase(Ease.Linear);
    }
    
    private void Confirm()
    {
        CAS.MobileAds.settings.userConsent = CAS.ConsentStatus.Undefined;
        Application.Quit();
    }

    private void Cancel()
    {
        CloseWarningPanel();
    }
}
