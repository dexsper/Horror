using System;
using UnityEngine;
using UnityEngine.UI;

public class HideVirtualKeyboard : InputField
{
    protected override void Start()
    {
        EditPlayerName.OnNameEditOpen += OnNameEditOpen;
        EditPlayerName.OnNameEditClose += OnNameEditClose;
        base.Start();
    }

    protected override void OnDestroy()
    {
        EditPlayerName.OnNameEditOpen -= OnNameEditOpen;
        EditPlayerName.OnNameEditClose -= OnNameEditClose;
        base.OnDestroy();
    }

    private void OnNameEditOpen()
    {
        keyboardType = (TouchScreenKeyboardType)(-1);
    }

    private void OnNameEditClose()
    {
        keyboardType = TouchScreenKeyboardType.Default;
    }
}
