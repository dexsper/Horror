using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeyboardRemoveButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _inputText;

    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(RemoveLetter);
    }

    private void RemoveLetter()
    {
        if(_inputText.text.Length >= 1)
            _inputText.text = _inputText.text.Remove(_inputText.text.Length - 1);
    }
}
