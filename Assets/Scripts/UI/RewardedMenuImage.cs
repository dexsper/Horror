using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class RewardedMenuImage : MonoBehaviour
{
    [SerializeField] private Button confirmButton,openButton,cancelButton;

    [SerializeField] private Transform menuImage;
    [SerializeField] private GameObject charactersContent;

    private void Awake()
    {
        CharacterWindowUI.OnRewardedSkinAdded += OnRewardedSkinAdded;
        
        confirmButton.onClick.AddListener(CloseMenu);
        openButton.onClick.AddListener(OpenMenu);
        cancelButton.onClick.AddListener(CloseMenu);

        //openButton.gameObject.SetActive(false);
    }

    public void SetButtonPosition(Vector3 position,Transform transform)
    {
        openButton.transform.parent = transform;
        openButton.transform.localPosition = position;
        openButton.gameObject.SetActive(true);
    }
    
    private void OnDestroy()
    {
        CharacterWindowUI.OnRewardedSkinAdded -= OnRewardedSkinAdded;
    }

    private void CloseMenu()
    {
        charactersContent.SetActive(true);
        menuImage.DOScale(0f, 0.25f).SetEase(Ease.Linear).From(1f);
    }

    private void OpenMenu()
    {
        charactersContent.SetActive(false);
        menuImage.DOScale(1f, 0.25f).SetEase(Ease.Linear).From(0f);
    }

    private void OnRewardedSkinAdded()
    {
        openButton.gameObject.SetActive(false);
    }
    
}
