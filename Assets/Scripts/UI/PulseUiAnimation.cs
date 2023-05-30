using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PulseUiAnimation : MonoBehaviour
{
    private void Start()
    {
        var Seq = DOTween.Sequence();
        Seq.Append(transform.DOScale(1.1f, 1.25f).SetEase(Ease.Linear).From(1f).OnComplete(delegate
        {
            transform.DOScale(1f, 1.25f).SetEase(Ease.Linear).From(1.1f);
        }));
        Seq.SetLoops(-1, LoopType.Yoyo);
    }

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(delegate
        {
            Application.OpenURL("https://play.google.com/store/apps/details?id=com.Khadiev.Siren.head.scary.horror.game");
        });
    }
}
