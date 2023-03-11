using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] private float animationDuration;
    [SerializeField] private TextMeshProUGUI roomNameText;
    public void OpenRoom(string text)
    {
        roomNameText.text = text;
        transform.DOScale(1, animationDuration).From(0).SetEase(Ease.Linear);
    }

    public void CloseRoom()
    {
        transform.DOScale(0, animationDuration).From(1).SetEase(Ease.Linear);
    }
}
