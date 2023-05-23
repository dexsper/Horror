/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;
using DG.Tweening;
using TMPro;

public class UI_InputWindow : MonoBehaviour {

    private static UI_InputWindow instance;

    [SerializeField] private Button_UI okBtn;
    [SerializeField] private Button_UI cancelBtn;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TMP_InputField inputField;

    private void Awake() {
        instance = this;
        Hide();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) {
            okBtn.ClickFunc();
        }
        if (Input.GetKeyDown(KeyCode.Escape)) {
            cancelBtn.ClickFunc();
        }
    }

    private void Show(string titleString, string inputString, string validCharacters, int characterLimit, Action onCancel, Action<string> onOk) {
        gameObject.SetActive(true);
        transform.DOScale(1f,0.25f).From(0f).SetEase(Ease.Linear).OnComplete(delegate
        {
            transform.SetAsLastSibling();
        });
        titleText.text = titleString;

        inputField.characterLimit = characterLimit;
        inputField.onValidateInput = (string text, int charIndex, char addedChar) => {
            return ValidateChar(validCharacters, addedChar);
        };

        inputField.text = inputString;
        inputField.Select();

        okBtn.ClickFunc = () => {
            Hide();
            onOk(inputField.text);
        };

        cancelBtn.ClickFunc = () => {
            Hide();
            onCancel();
        };
    }

    private void Hide()
    {
        transform.DOScale(0f, 0.25f).From(1f).SetEase(Ease.Linear).OnComplete(delegate
        {
            gameObject.SetActive(false);
        });
    }

    private char ValidateChar(string validCharacters, char addedChar) {
        if (validCharacters.IndexOf(addedChar) != -1) {
            // Valid
            return addedChar;
        } else {
            // Invalid
            return '\0';
        }
    }

    public static void Show_Static(string titleString, string inputString, string validCharacters, int characterLimit, Action onCancel, Action<string> onOk) {
        instance.Show(titleString, inputString, validCharacters, characterLimit, onCancel, onOk);
    }

    public static void Show_Static(string titleString, int defaultInt, Action onCancel, Action<int> onOk) {
        instance.Show(titleString, defaultInt.ToString(), "0123456789-", 20, onCancel, 
            (string inputText) => {
                // Try to Parse input string
                if (int.TryParse(inputText, out int _i)) {
                    onOk(_i);
                } else {
                    onOk(defaultInt);
                }
            }
        );
    }
}
