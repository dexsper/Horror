using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class EmojiController : MonoBehaviour
{
    [SerializeField] private List<Vector3> endPositions;
    [SerializeField] private List<Button> emojiButtons;
    [SerializeField] private List<Material> emojiButtonsMaterials;

    private Button _openButton;
    private bool _isOpen;
    
    public event Action<Material> OnEmojiPlayed;

    private void Awake()
    {
        _openButton = GetComponent<Button>();
        
        _openButton.onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        if (!_isOpen)
        {
            Open();
        }
        else
        {
            Close();
        }
    }

    private void Open()
    {
        _isOpen = true;
        for (int i = 0; i < emojiButtons.Count; i++)
        {
            emojiButtons[i].transform.DOLocalMove(endPositions[i], 0.2f).SetEase(Ease.Linear);
            emojiButtons[i].transform.DOScale(1f, 0.2f).SetEase(Ease.Linear);
        }
    }

    private void Close()
    {
        _isOpen = false;
        for (int i = 0; i < emojiButtons.Count; i++)
        {
            emojiButtons[i].transform.DOLocalMove(Vector3.zero, 0.2f).SetEase(Ease.Linear);
            emojiButtons[i].transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.Linear);
        }
    }
}
