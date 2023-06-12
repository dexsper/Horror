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
        confirmButton.onClick.AddListener(CloseMenu);
        openButton.onClick.AddListener(OpenMenu);
        cancelButton.onClick.AddListener(CloseMenu);
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

}