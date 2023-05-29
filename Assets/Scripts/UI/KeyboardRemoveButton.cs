using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeyboardRemoveButton : MonoBehaviour
{
    [SerializeField] private UI_InputWindow inputWindow;

    private TMP_InputField inputField => inputWindow.InputField;

    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(RemoveLetter);
    }

    private void RemoveLetter()
    {
        if(inputField.text.Length >= 1)
            inputField.text = inputField.text.Remove(inputField.text.Length - 1);
    }
}
