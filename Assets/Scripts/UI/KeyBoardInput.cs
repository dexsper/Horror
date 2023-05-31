using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeyBoardInput : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _inputText;
    
    private Button _button;
    
    private string _letter;

    private void Awake()
    {
        _letter = GetComponentInChildren<TextMeshProUGUI>().text;
        _button = GetComponent<Button>();
        
        _button.onClick.AddListener(AddLetter);
    }

    private void AddLetter()
    {
        _inputText.text += _letter;
    }
}
